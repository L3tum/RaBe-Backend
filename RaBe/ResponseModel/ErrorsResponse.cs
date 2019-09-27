using RaBe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaBe.ResponseModel
{
    public class ErrorsResponse
    {
        public long roomId;
        public string roomName;
        public List<Fehler> errors = new List<Fehler>();
    }
}
