using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class LehrerRaum
    {
        public long Id { get; set; }
        public bool Betreuer { get; set; }
        public long LehrerId { get; set; }
        public long RaumId { get; set; }

        [JsonIgnore]
        public virtual Lehrer Lehrer { get; set; }
        [JsonIgnore]
        public virtual Raum Raum { get; set; }
    }
}
