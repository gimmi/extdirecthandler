using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtDirectHandler;

namespace MultipleDirectApi
{
    /// <summary>
    /// Demo ApiManager that contains 2 APIs: PublicApi and PrivateApi
    /// </summary>
    public class DemoApiManager : DirectApiManager
    {
        protected override IEnumerable<Type> GetAPIs()
        {
            return new []
            { 
                typeof(PublicApi), 
                typeof(PrivateApi)
            };
        }
    }
}