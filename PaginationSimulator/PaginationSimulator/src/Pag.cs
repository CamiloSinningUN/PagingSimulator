using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public class Pag
    {
        public Pag(int marco, bool valid, bool dirty, int time, int index)
        {
            this.marco = marco;
            this.valid = valid;
            this.dirty = dirty;
            this.time = time;
            this.index = index;
        }

        public int marco { get; set; }
        public bool valid { get; set; }
        public bool dirty { get; set; }
        public int time { get; set; }
        public int index { get; set; }

    }
}
