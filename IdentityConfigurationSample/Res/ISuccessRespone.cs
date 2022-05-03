using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Res
{
    public interface ISuccessResponse<T>
    {
         string status { get; set; }
         T data { get; set; }
    }
}
