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

        protected IEnumerable<Type> APIs { get; set; }

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
            APIs = this.GetAPIs();
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
                var apiName = request.QueryString["api"];
                if (!String.IsNullOrEmpty(apiName))
                {
                    Type apiType = APIs.Where(t => t.Name == apiName).FirstOrDefault();
                    if (apiType != null)
                    {
                        var handler = CreateInstance(apiType);
                        if (handler is IHttpHandler)
                        {
                            (handler as IHttpHandler).ProcessRequest(app.Context);
                            response.End();
                        }
                        else
                        {
                            response.Write(String.Format("'{0}' is not an IHttpHandler", apiType.Name));
                            response.End();
                        }
                    }
                    else
                    {
                        response.Write(String.Format("Unknown API: '{0}'", apiName));
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
        /// Returns collection of APIs. Should be overriden in descendands.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<Type> GetAPIs();

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