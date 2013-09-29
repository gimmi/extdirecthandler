using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExtDirectHandler
{
    /// <summary>
    /// Api Manager for managing multiple DirectHttpHandler instances.
    /// </summary>
    public abstract class DirectApiManager : IHttpModule 
    {
        protected string Path
        {
            get
            {
                return "/rpc";
            }
        }

        protected IDictionary<string, Type> Configuration { get; set; }

        protected HttpApplication Context { get; set; }

        public void Dispose()
        {
            Context.BeginRequest -= Context_BeginRequest;
        }

        /// <summary>
        /// Manager initialization.
        /// </summary>
        /// <example>
        /// Usage:
        /// DynamicModuleUtility.RegisterModule(typeof(ApiManager));
        /// </example>
        public void Init(HttpApplication context)
        {
            Configuration = this.GetConfiguration();
            Context = context;
            context.BeginRequest += Context_BeginRequest;
        }

        /// <summary>
        /// Request processing.
        /// </summary>
        protected virtual void Context_BeginRequest(object sender, EventArgs e)
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
                        var handler = CreateInstance(type);
                        if (handler is IHttpHandler)
                        {
                            (handler as IHttpHandler).ProcessRequest(app.Context);
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
        }

        /// <summary>
        /// Retreiving dictionary of API's names to their types. Should be overriden in descendands.
        /// </summary>
        /// <returns></returns>
        protected abstract IDictionary<string, Type> GetConfiguration();

        /// <summary>
        /// Type instance creation. Can be overriden for custom dependency injection.
        /// </summary>
        /// <param name="type">Type to create</param>
        /// <returns>Created instance</returns>
        protected virtual object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}