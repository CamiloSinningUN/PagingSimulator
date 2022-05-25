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

        public void ExInstruc(Instruc instruc, int time)
        {
            Console.WriteLine($"instruc: (dir={instruc.dir}, lec={instruc.lec})");
            int pag = instruc.dir / tamMarco;
            int marco = tablaPag[pag].marco;
            Console.WriteLine(marco);
            bool swapIn, swapOut;
            if (marco == -1)
            {
                marco = findMarcoLibre();
                if (marco == -1)
                {
                    marco = marcosUsage.ElementAt(0);
                    Pag pagVictim = getPageFromMarco(marco);
                    
                    swapOut = pagVictim.dirty;
                    pagVictim.valid = pagVictim.dirty = false;

                    printTablePag();

                    numReemp++;
                    Console.WriteLine($"Marco tomado: {marco}, víctima: {pagVictim.marco}, swap_out: {swapOut}");
                    if (alg == FIFO)
                        marcosUsage.RemoveFirst();
                }
                else
                {
                    Console.WriteLine($"Marco libre: {marco}");
                    swapOut = false;
                }
                tablaPag[pag].marco = marco;
                tablaPag[pag].dirty = !instruc.lec;
                tablaPag[pag].valid = true;
                tablaPag[pag].time = time + 1;

                swapIn = true;
                numFallosPag++;
                marcos[marco] = MARCO_CON_PAG;

                if(alg == FIFO)
                    marcosUsage.AddLast(marco);
            }
            else
            {
                swapIn = swapOut = false;
                Console.WriteLine($"No fallo de pag: marco={marco}");
            }

            if(alg == LRU)
            {
                marcosUsage.Remove(marco);
                marcosUsage.AddLast(marco);
            }
            int dirFis = marco * tamMarco + instruc.dir % tamProc;
            Console.WriteLine($"pag={pag}, marco={marco}, dirFis={dirFis}, swapIn={swapIn}, swapOut={swapOut}");
            Console.WriteLine("");
            printTablePag();
            printMarcosUsage();
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

        public void printTablePag()
        {
            Console.WriteLine("TablaPag:");
            foreach (Pag p in tablaPag)
                Console.WriteLine($"marco={p.marco}, valid={p.valid}, dirty={p.dirty}");
            Console.WriteLine("");
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
        private byte[] marcos;
        public Pag[] tablaPag;
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
