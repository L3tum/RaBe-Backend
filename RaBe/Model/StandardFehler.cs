using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class StandardFehler
    {
        public long Id { get; set; }
        public long Status { get; set; }
        public string Beschreibung { get; set; }
        public string Titel { get; set; }
        public long KategorieId { get; set; }

        public virtual Kategorie Kategorie { get; set; }
    }
}
