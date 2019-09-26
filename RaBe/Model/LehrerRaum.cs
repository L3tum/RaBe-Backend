using System;
using System.Collections.Generic;

namespace RaBe
{
    public partial class LehrerRaum
    {
        public long Id { get; set; }
        public int Betreuer { get; set; }
        public long LehrerId { get; set; }
        public long RaumId { get; set; }

        public virtual Lehrer Lehrer { get; set; }
        public virtual Raum Raum { get; set; }
    }
}
