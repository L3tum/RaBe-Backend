using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class Arbeitsplatz
    {
        public Arbeitsplatz()
        {
            Fehler = new HashSet<Fehler>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long RaumId { get; set; }
        public long Position { get; set; }

        [JsonIgnore]
        public virtual Raum Raum { get; set; }
        [JsonIgnore]
        public virtual ICollection<Fehler> Fehler { get; set; }
    }
}
