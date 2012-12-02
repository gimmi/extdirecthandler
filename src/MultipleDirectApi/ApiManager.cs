using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultipleDirectActions
{
    /// <summary>
    /// Api Manager
    /// </summary>
    /// <remarks>
    /// see: MultipleDirectActions\Properties\AssemblyInfo.cs
    /// </remarks>
    public class ApiManager : IHttpModule 
    {
        protected string Path
        {
            get
            {
                return "/rpc";
            }
        }

        protected IDictionary<string, Type> Configuration { get; set; }

        public static void Start()
        {
            // http://blog.davidebbo.com/2011/02/register-your-http-modules-at-runtime.html
            // Register our module
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(ApiManager));

            // If you want to use dependency injection read this article
            // http://haacked.com/archive/2011/06/02/dependency-injection-with-asp-net-httpmodules.aspx
        }

        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            Configuration = this.GetConfiguration();

            context.BeginRequest += (sender, e) =>
            {
                var app = ((HttpApplication)sender);                
                var request = app.Request;
                var response = app.Response;

                if (request.Url.AbsolutePath == this.Path)
                {
                    var api = request.QueryString["api"];
                    if (!String.IsNullOrEmpty(api))
                    {
                        if (Configuration.ContainsKey(api))
                        {
                            Type type = Configuration[api];
                            var handler = Activator.CreateInstance(type) as IHttpHandler;
                            if (handler != null)
                            {
                                handler.ProcessRequest(app.Context);
                                response.End();
                            }
                            else
                            {
                                response.Write(String.Format("'{0}' is not an IHttpHandler", type.Name));
                                response.End();
                            }
                        }
                        else
                        {
                            response.Write(String.Format("Unknown API: '{0}'", api));
                            response.End();
                        }
                    }
                    else
                    {
                        response.Write(String.Format("API is not defined. Use {0}/?api=ApiName", Path));
                        response.End();
                    }
                }                
            };
        }

        protected IDictionary<string, Type> GetConfiguration()
        {
            var result = new Dictionary<string, Type>();
            result.Add("PublicApi", typeof(PublicApi));
            result.Add("PrivateApi", typeof(PrivateApi));
            return result;
        }
    }
}