using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public class Salida
    {
        public Salida(int numPag, int numMarco, int dirFis, bool swapIn, bool swapOut)
        {
            this.numPag = numPag;
            this.numMarco = numMarco;
            this.dirFis = dirFis;
            this.swapIn = swapIn;
            this.swapOut = swapOut;
        }

        public int numPag;
        public int numMarco;
        public int dirFis;
        public bool swapIn;
        public bool swapOut;

    }
}
