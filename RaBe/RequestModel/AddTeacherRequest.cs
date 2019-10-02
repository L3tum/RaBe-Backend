using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaBe.RequestModel
{
    public class AddTeacherRequest
    {
        public string name;
        public string email;
        public string password;
        public bool admin;
    }
}
