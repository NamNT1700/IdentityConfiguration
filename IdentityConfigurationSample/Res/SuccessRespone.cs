using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Res
{
    public  class SuccessRespone<T>
    {
        public  string status { get  { return "SUCCESS"; } set { } }
        public  T data { get; set; }
    }
}
