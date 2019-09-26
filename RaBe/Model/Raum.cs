using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaBe.Model
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

        [JsonIgnore]
        public virtual ICollection<Arbeitsplatz> Arbeitsplatz { get; set; }

        [JsonIgnore]
        public virtual ICollection<LehrerRaum> LehrerRaum { get; set; }
    }
}
