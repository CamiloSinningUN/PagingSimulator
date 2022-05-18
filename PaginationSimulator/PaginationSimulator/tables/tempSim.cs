using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.tables
{
    public class tempSim
    {
        public string tamMarco { get; set; }
        public string tamSO { get; set; }
        public string tamProc { get; set; }
        public string tamMP { get; set; }

        public tempSim(string tamMarco, string tamSO, string tamProc, string tamMem)
        {
            this.tamMarco = tamMarco;
            this.tamSO = tamSO;
            this.tamProc = tamProc;
            this.tamMP = tamMem;
        }
    }
}
