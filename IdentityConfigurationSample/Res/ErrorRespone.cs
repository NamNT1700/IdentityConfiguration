using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Res
{
    public class ErrorRespone : IErrorResponse
    {
        public string status { get { return "ERROR"; } set { } }

        public List<string> Description { get; set; }
        
    }
}
