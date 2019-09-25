using System;
using System.Collections.Generic;

namespace RaBe
{
    public partial class Raum
    {
        public Raum()
        {
            Arbeitsplatz = new HashSet<Arbeitsplatz>();
            LehrerRaum = new HashSet<LehrerRaum>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Arbeitsplatz> Arbeitsplatz { get; set; }
        public virtual ICollection<LehrerRaum> LehrerRaum { get; set; }
    }
}
