using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public class InstOutput
    {
        public InstOutput(int dirLog, int dirFis, bool lec, int pag, int marco, bool swapIn, bool swapOut, string bitac)
        {
            this.dirLog = dirLog;
            this.dirFis = dirFis;
            this.lec = lec;
            this.pag = pag;
            this.marco = marco;
            this.swapIn = swapIn;
            this.swapOut = swapOut;
            this.bitac = bitac;
        }

        public int dirLog;
        public int dirFis;
        public bool lec;
        public int pag;
        public int marco;
        public bool swapIn;
        public bool swapOut;
        public string bitac;
    }
}
