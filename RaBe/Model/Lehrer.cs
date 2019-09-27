﻿using Newtonsoft.Json;
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

        [JsonIgnore]
        public string Password { get; set; }
        public long PasswordGeaendert { get; set; }
        public long Blocked { get; set; }
        public long Administrator { get; set; }

        [JsonIgnore]
        public string Token { get; set; }

        [JsonIgnore]
        public virtual ICollection<LehrerRaum> LehrerRaum { get; set; }
    }
}
