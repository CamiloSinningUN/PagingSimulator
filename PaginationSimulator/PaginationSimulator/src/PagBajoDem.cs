using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public partial class PagBajoDem
    {
        public PagBajoDem(int tamMarco, int tamProc, int tamSO, int tamMP, string alg)
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

            this.alg = alg;
            this.tamMarco = tamMarco;
            this.tamProc = tamProc;
            this.tamSO = tamSO;
            this.tamMP = tamMP;
            this.numMarcos = tamMP / tamMarco;
            this.numMarcosSO = (tamSO + tamMarco - 1) / tamMarco;
            this.marcos = new byte[numMarcos];
            initMarcosSO();
            this.numPagProc = (tamProc + tamMarco - 1) / tamMarco;
            this.tablaPag = new TablaPagRow[numPagProc];
            initTablaPag();
            this.allMarcosLlenos = false;
            this.marcosLRU = new LinkedList<int>();
            this.numFallosPag = 0;
            this.numReemp = 0;
        }
        private void CheckInput(int tamMarco, int tamProc, int tamSO, int tamMP)
        {
            // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            if ((tamMarco & (tamMarco - 1)) != 0)
                throw new PagBajoDemException($"El tamaño de cada marco ({tamMarco} bytes) debe ser potencia de 2", MARCO_POW_OF_2_EXCEPTION);
            if ((tamMP & (tamMP - 1)) != 0)
                throw new PagBajoDemException($"El tamaño de la MP ({tamSO} bytes) debe ser potencia de 2", MP_POW_OF_2_EXCEPTION);
            if (tamSO > tamMP)
                throw new PagBajoDemException($"El tamaño del SO ({tamSO} bytes) no puede ser mayor al de la MP ({tamMP} bytes)", SO_EXCEPTION);
            if (tamProc > tamMP)
                throw new PagBajoDemException($"El tamaño del proceso ({tamProc} bytes) no puede ser mayor al de la MP ({tamMP} bytes)", PROC_EXCEPTION);
            if (tamMarco > tamMP)
                throw new PagBajoDemException($"El tamaño de cada marco ({tamMarco} bytes) no puede ser mayor al de la MP ({tamMP} bytes)", MARCO_EXCEPTION);
            int sum = tamSO + tamProc;
            if (sum > tamMP)
                throw new PagBajoDemException($"El tamaño SO y el del proceso ({sum} bytes) no pueden sumar más que el de la MP ({tamMP} bytes)", SO_AND_PROC_EXCEPTION);
        }
        private void initMarcosSO()
        {
            for (int i = 0; i < numMarcosSO; i++)
                marcos[i] = MARCO_CON_SO;
        }
        private void initTablaPag()
        {
            for (int i = 0; i < numPagProc; i++)
                tablaPag[i] = new TablaPagRow(-1, false, false, 0);
        }
        private void initRestoMarcos(bool[] marcosInit)
        {
            for (int i = numMarcosSO; i < numMarcos; i++)
                marcos[i] = marcosInit[i] ? MARCO_FREE : MARCO_OCCUP_PREV;
        }
        public void Start(bool[] marcosInit)
        {
            initRestoMarcos(marcosInit);
        }
        public void ExInstruc(Instruc instruc, int time)
        {
            Console.WriteLine($"instruc: (dir={instruc.dir}, lec={instruc.lec})");
            int pag = instruc.dir / tamMarco;
            int marco = tablaPag[pag].marco;
            bool swapIn, swapOut;
            if (marco == -1)
            {
                marco = findMarcoLibre();
                if (marco == -1)
                {
                    marco = getMarcoLRU();
                    TablaPagRow pagVictim = getPageFromMarco(marco);
                    swapOut = pagVictim.dirty;
                    numReemp++;
                    Console.WriteLine($"Marco tomado: {marco}, víctima: {pagVictim}");
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
            }
            else
            {
                swapIn = swapOut = false;
                Console.WriteLine($"No fallo de pag (marco: {marco})");
            }

            marcosLRU.Remove(marco);
            marcosLRU.AddLast(marco);
            int dirFis = marco * tamMarco + instruc.dir % tamProc;
            Console.WriteLine($"pag={pag}, marco={marco}, dirFis={dirFis}, swapIn={swapIn}, swapOut={swapOut}");
            Console.WriteLine("");
            printTablePag();
            printMarcosLRU();
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
        private int getMarcoLRU()
        {
            return marcosLRU.ElementAt(0);
        }

        private TablaPagRow getPageFromMarco(int marco)
        {
            for (int i = 0; i < tablaPag.Length; i++) if (tablaPag[i].marco == marco) return tablaPag[i];
            return null;
        }
        public void printMarcos()
        {
            Console.WriteLine("Marcos:");
            for (int i = 0; i < marcos.Length; i++)
                Console.WriteLine(i + " " + marcos[i]);
            Console.WriteLine("");
        }
        public void printMarcosLRU()
        {
            Console.WriteLine("MarcosLRU:");
            foreach (int m in marcosLRU)
                Console.WriteLine(m);
            Console.WriteLine("");
        }

        public void printTablePag()
        {
            Console.WriteLine("TablaPag:");
            foreach (TablaPagRow p in tablaPag)
                Console.WriteLine($"marco={p.marco}, valid={p.valid}, dirty={p.dirty}");
            Console.WriteLine("");
        }
        class PagBajoDemException : Exception
        {
            public PagBajoDemException(string message, byte type) : base(message)
            {
                this.type = type;
            }
            public byte type;
        }
    }

    public partial class PagBajoDem
    {
        // public const int MAX_TAM_MARCO = 8192;
        // public const int MIN_TAM_MARCO = 0; //512
        public string alg;
        public int tamMarco;
        public int tamProc;
        public int tamSO;
        public int tamMP;
        public int numMarcos;
        public int numMarcosSO;
        public int numPagProc;
        public int numFallosPag;
        public int numReemp;
        private byte[] marcos;
        private TablaPagRow[] tablaPag;
        private LinkedList<int> marcosLRU;
        private bool allMarcosLlenos;
        // Tipos de error
        public const byte MP_POW_OF_2_EXCEPTION = 0;
        public const byte MARCO_POW_OF_2_EXCEPTION = 1;
        public const byte MP_EXCEPTION = 2;
        public const byte SO_EXCEPTION = 3;
        public const byte PROC_EXCEPTION = 4;
        public const byte MARCO_EXCEPTION = 5;
        public const byte SO_AND_PROC_EXCEPTION = 6;
        // Estados de marco
        public const byte MARCO_FREE = 0;
        public const byte MARCO_CON_PAG = 1;
        public const byte MARCO_OCCUP_PREV = 2;
        public const byte MARCO_CON_SO = 3;
    }

}
