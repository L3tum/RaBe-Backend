using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class Fehler
    {
        public long Id { get; set; }
        public long Status { get; set; }
        public string Beschreibung { get; set; }
        public string Titel { get; set; }
        public long ArbeitsplatzId { get; set; }
        public long KategorieId { get; set; }

        [JsonIgnore]
        public virtual Arbeitsplatz Arbeitsplatz { get; set; }
        [JsonIgnore]
        public virtual Kategorie Kategorie { get; set; }
    }
}
