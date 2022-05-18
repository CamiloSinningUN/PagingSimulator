using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public class Instruc
    {
        public Instruc(int dir, bool lec)
        {
            this.dir = dir;
            this.lec = lec;
        }
        public int dir { get; set; }
        public bool lec { get; set; }

    }
}
