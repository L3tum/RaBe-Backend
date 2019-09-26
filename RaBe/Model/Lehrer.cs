using System;
using System.Collections.Generic;

namespace RaBe.Model
{
    public partial class Lehrer
    {
        public Lehrer()
        {
            LehrerRaum = new HashSet<LehrerRaum>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordGeaendert { get; set; }
        public string Token { get; set; }

        public virtual ICollection<LehrerRaum> LehrerRaum { get; set; }
    }
}
