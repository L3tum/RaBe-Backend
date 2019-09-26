using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class Kategorie
    {
        public Kategorie()
        {
            Fehler = new HashSet<Fehler>();
            StandardFehler = new HashSet<StandardFehler>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Fehler> Fehler { get; set; }
        [JsonIgnore]
        public virtual ICollection<StandardFehler> StandardFehler { get; set; }
    }
}
