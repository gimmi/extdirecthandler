using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MultipleDirectApi.MVC4
{
    using Direct.Api;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new Route("rpc/Public{api}", new Direct.DirectRouteHandler<PublicApi>()));
            routes.Add(new Route("rpc/Admin{api}", new Direct.DirectRouteHandler<AdminApi>()));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}