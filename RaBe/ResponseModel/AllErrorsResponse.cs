using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaBe.ResponseModel
{
    public class AllErrorsResponse
    {
        public Dictionary<long, ErrorsResponse> rooms = new Dictionary<long, ErrorsResponse>();
    }
}
