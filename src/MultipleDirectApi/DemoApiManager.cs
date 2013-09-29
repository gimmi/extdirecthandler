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
        protected override IDictionary<string, Type> GetConfiguration()
        {
            var result = new Dictionary<string, Type>()
            { 
                { "PublicApi", typeof(PublicApi) },
                { "PrivateApi", typeof(PrivateApi) }
            };
            return result;
        }
    }
}