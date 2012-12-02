using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;

namespace MultipleDirectActions
{
    public class PrivateApi : DirectHttpHandler
    {
        protected override IMetadata GetMetadata()
        {
            return new ReflectionConfigurator()
                .SetNamespace("Server")
                .SetId(this.GetType().Name)
                .RegisterType(typeof(PrivateAction));
        }
    }
}