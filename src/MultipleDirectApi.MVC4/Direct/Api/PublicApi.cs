using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtDirectHandler;
using ExtDirectHandler.Configuration;

namespace MultipleDirectApi.MVC4.Direct.Api
{
    public class PublicApi: DirectHttpHandler
    {
        protected override IMetadata GetMetadata()
        {
            return new ReflectionConfigurator()
            {
                Namespace = "Server",
                Id = this.GetType().Name
            }
            .RegisterType<Actions.ProductsAction>()
            .RegisterType<Actions.OrdersAction>();
        }
    }
}