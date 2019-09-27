using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaBe.ResponseModel
{
    public class LoginResponse
    {
        public bool passwordInvalid;
        public bool passwordChanged;
        public bool isBlocked;

        public LoginResponse(bool invalid, bool changed, bool blocked)
        {
            passwordInvalid = invalid;
            passwordChanged = changed;
            isBlocked = blocked;
        }
    }
}
