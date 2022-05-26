using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PaginationSimulator.src
{
    public partial class PagBajoDem
    {
        public PagBajoDem(int tamMarco, int tamProc, int tamSO, int tamMP)
        {
            try
            {
                CheckInput(tamMarco, tamProc, tamSO, tamMP);
                Console.WriteLine("Input válido");
            }
            catch (PagBajoDemException e)
            {
                throw e;
            }

            this.alg = LRU;
            this.tamMarco = tamMarco;
            this.tamProc = tamProc;
            this.tamSO = tamSO;
            this.tamMP = tamMP;
            this.marcos = new byte[numMarcos];
            initMarcosSO();
            this.numPagProc = (tamProc + tamMarco - 1) / tamMarco;
            this.tablaPag = new Pag[numPagProc];
            initTablaPag();
            this.allMarcosLlenos = false;
            this.marcosUsage = new LinkedList<int>();
            this.numFallosPag = 0;
            this.numReemp = 0;
        }
        private void CheckInput(int tamMarco, int tamProc, int tamSO, int tamMP)
        {
            // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            //if ((tamMP & (tamMP - 1)) != 0)
            //    throw new PagBajoDemException($"Tamaño de la MP ({tamSO}) debe ser potencia de 2.", PagBajoDemException.MP_POW_OF_2_EXCEPTION);
            if ((tamMarco & (tamMarco - 1)) != 0)
                throw new PagBajoDemException($"Tamaño de marco ({tamMarco}) debe ser potencia de 2.", PagBajoDemException.MARCO_POW_OF_2_EXCEPTION);
            if (tamMP <= 0)
                throw new PagBajoDemException($"Tamaño de la MP ({tamMP}) debe ser mayor a 0.", PagBajoDemException.MP_EXCEPTION);
            if (tamSO > tamMP || tamSO < 0)
                throw new PagBajoDemException($"Tamaño del SO ({tamSO}) debe estar en [0, {tamMP}].", PagBajoDemException.SO_EXCEPTION);
            if (tamProc > tamMP || tamProc < 0)
                throw new PagBajoDemException($"Tamaño del proceso ({tamProc}) debe estar en (0, {tamMP}].", PagBajoDemException.PROC_EXCEPTION);
            if (tamMarco > tamMP || tamMarco <= 0)
                throw new PagBajoDemException($"Tamaño marco ({tamMarco}) debe estar en (0, {tamMP}].", PagBajoDemException.MARCO_EXCEPTION);
            

            this.numMarcos = tamMP / tamMarco;
            if(numMarcos > MAX_MARCOS)
                throw new PagBajoDemException($"Por problemas de rendimiento, no se permiten más de ({MAX_MARCOS} marcos).", PagBajoDemException.NUM_MARCOS_EXCEPTION);

            this.numMarcosProc = (tamProc + tamMarco - 1) / tamMarco;
            this.numMarcosSO = (tamSO + tamMarco - 1) / tamMarco;

            int sum = numMarcosSO + numMarcosProc;
            if (sum > numMarcos)
                throw new PagBajoDemException($"Marcos para SO y proceso ({numMarcosSO} + {numMarcosProc} = {sum}) superan marcos totales ({numMarcos}).", PagBajoDemException.SO_AND_PROC_EXCEPTION);
        }

        private void initMarcosSO()
        {
            for (int i = 0; i < numMarcosSO; i++)
                marcos[i] = MARCO_CON_SO;
        }

        private void initTablaPag()
        {
            for (int i = 0; i < numPagProc; i++)
                tablaPag[i] = new Pag(-1, false, false, -1, i);
        }
        
        public void InitMarcos(bool[] marcosInit)
        {
            for (int i = numMarcosSO; i < numMarcos; i++)
                marcos[i] = marcosInit[i] ? MARCO_FREE : MARCO_OCCUP_PREV;
        }

        public InstOutput ExInstruc(Instruc instruc, int time)
        {
            string bit = "";
            bit += $"Dirección = {instruc.dir}, {(instruc.lec ? "lectura" : "escritura")}" + Environment.NewLine + "--------------------------------" + Environment.NewLine;
            int pag = instruc.dir / tamMarco;
            int marco = tablaPag[pag].marco;
            Console.WriteLine(marco);
            bool swapIn, swapOut;
            if (!tablaPag[pag].valid)
            {
                bit += "Fallo de página" + Environment.NewLine;
                marco = findMarcoLibre();
                if (marco == -1)
                {
                    bit += "No se encontró marco libre" + Environment.NewLine;
                    marco = marcosUsage.ElementAt(0);
                    Pag pagVictim = getPageFromMarco(marco);

                    swapOut = pagVictim.dirty;
                    pagVictim.valid = pagVictim.dirty = false;

                    numReemp++;
                    bit += $"Marco tomado: {marco}, página víctima: {pagVictim.index}, swap out: {swapOut}" + Environment.NewLine;
                    if (alg == FIFO)
                        marcosUsage.RemoveFirst();
                }
                else
                {
                    bit += $"Marco libre tomado = {marco}" + Environment.NewLine;
                    swapOut = false;
                }
                tablaPag[pag].marco = marco;

                tablaPag[pag].valid = true;
                tablaPag[pag].time = time + 1;

                swapIn = true;
                numFallosPag++;
                marcos[marco] = MARCO_CON_PAG;

                if (alg == FIFO)
                    marcosUsage.AddLast(marco);
            }
            else
            {
                swapIn = swapOut = false;
                bit += $"No hubo fallo de página: marco = {marco}" + Environment.NewLine;
            }

            if (alg == LRU)
            {
                marcosUsage.Remove(marco);
                marcosUsage.AddLast(marco);
            }

            if (!instruc.lec) tablaPag[pag].dirty = true;

            int dirFis = marco * tamMarco + (instruc.dir % tamMarco);
            bit += $"Resumen: \n página = {pag} \n marco = {marco} \n dirección física = {dirFis} \n swap in = {swapIn} \n swap out = {swapOut}" + Environment.NewLine;
            bit += Environment.NewLine;
            bit += printTablePag();
            printMarcosUsage();

            return new InstOutput(instruc.dir, dirFis, instruc.lec, pag, marco, swapIn, swapOut, bit);
        }

        private int findMarcoLibre()
        {
            if (allMarcosLlenos) return -1;
            int marco = -1;
            for (int i = numMarcosSO; i < numMarcos && marco == -1; i++)
                if (marcos[i] == MARCO_FREE)
                    marco = i;
            if (marco == -1) allMarcosLlenos = true;
            return marco;
        }

        private Pag getPageFromMarco(int marco)
        {
            for (int i = 0; i < tablaPag.Length; i++)
                if (tablaPag[i].valid && tablaPag[i].marco == marco)
                    return tablaPag[i];
            return null;
        }
        public void printMarcos()
        {
            Console.WriteLine("Marcos:");
            for (int i = 0; i < marcos.Length; i++)
                Console.WriteLine(i + " " + marcos[i]);
            Console.WriteLine("");
        }
        public void printMarcosUsage()
        {
            Console.WriteLine("Uso de los marcos:");
            foreach (int m in marcosUsage)
                Console.WriteLine(m);
            Console.WriteLine("");
        }

        public string printTablePag()
        {
            string bit = "";
            bit += "TABLA DE PÁGINAS:"+Environment.NewLine;
            bit += "MARCO\tVALID\tDIRTY"+Environment.NewLine;
            bit += "------\t------\t------" + Environment.NewLine;
            foreach (Pag p in tablaPag)
                
                bit += $"{(p.marco==-1 ? "-" : p.marco+"")}\t{(p.valid ? "1" : "0")}\t{(p.dirty ? "1" : "0")}"+Environment.NewLine;
            bit += "";
            return bit;
        }

        public ObservableCollection<Pag> getTablePag()
        {
            ObservableCollection<Pag> temp = new ObservableCollection<Pag>();
            foreach (Pag p in tablaPag)
                temp.Add(p);
            return temp;
        }

        public void Clear()
        {
            this.numFallosPag = 0;
            this.numReemp = 0;
            this.marcos = new byte[numMarcos];
            this.tablaPag = new Pag[numPagProc];
            initTablaPag();
            this.marcosUsage = new LinkedList<int>();
            this.allMarcosLlenos = false;
        }
    }

    public partial class PagBajoDem
    {
        public class PagBajoDemException : Exception
        {
            public PagBajoDemException(string Message, byte Type) : base(Message)
            {
                this.Type = Type;
            }
            public byte Type;

            // Tipos de error
            public const byte MP_POW_OF_2_EXCEPTION = 0;
            public const byte MARCO_POW_OF_2_EXCEPTION = 1;
            public const byte MP_EXCEPTION = 2;
            public const byte SO_EXCEPTION = 3;
            public const byte PROC_EXCEPTION = 4;
            public const byte MARCO_EXCEPTION = 5;
            public const byte SO_AND_PROC_EXCEPTION = 6;
            public const byte NUM_MARCOS_EXCEPTION = 7;
        }
        // public const int MAX_TAM_MARCO = 8192;
        // public const int MIN_TAM_MARCO = 0; //512
        public byte alg { get; set; }
        public int tamMarco { get; set; }
        public int tamProc { get; set; }
        public int tamSO { get; set; }
        public int tamMP { get; set; }
        public int numMarcos;
        public int numMarcosProc;
        public int numMarcosSO;
        public int numPagProc;
        public int numFallosPag;
        public int numReemp;
        public byte[] marcos { get; set; }
        public Pag[] tablaPag { get; set; }
        private LinkedList<int> marcosUsage;
        private bool allMarcosLlenos;

        private const int MAX_MARCOS = 512;

        // Algoritmos
        public const byte FIFO = 0;
        public const byte LRU = 1;

        // Estados de marco
        public const byte MARCO_FREE = 0;
        public const byte MARCO_CON_PAG = 1;
        public const byte MARCO_OCCUP_PREV = 2;
        public const byte MARCO_CON_SO = 3;
    }

}
