using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaginationSimulator.src
{
    public class TablaPagRow
    {
        public TablaPagRow(int marco, bool valid, bool dirty, int time)
        {
            this.marco = marco;
            this.valid = valid;
            this.dirty = dirty;
            this.time = time;
        }

        public int marco;
        public bool valid;
        public bool dirty;
        public int time;

    }
}
