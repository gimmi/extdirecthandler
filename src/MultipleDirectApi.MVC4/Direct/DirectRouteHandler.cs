using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ExtDirectHandler;

namespace MultipleDirectApi.MVC4.Direct
{
    public class DirectRouteHandler<T> : IRouteHandler
        where T : DirectHttpHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}