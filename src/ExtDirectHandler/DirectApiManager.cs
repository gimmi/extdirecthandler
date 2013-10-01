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
        /// <summary>
        /// Path for RPC requests
        /// </summary>
        protected virtual string RpcPath
        {
            get
            {
                return "/rpc";
            }
        }

        /// <summary>
        /// Default API name when no API is specified ('DefaultApi').
        /// </summary>
        protected virtual string DefaultApiName
        {
            get
            {
                return "DefaultApi";
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

            if ((request.ApplicationPath == "/" && request.Url.AbsolutePath == this.RpcPath)
                || (request.Url.AbsolutePath == request.ApplicationPath + this.RpcPath))
            {
                var apiName = request.QueryString["api"];

                if (String.IsNullOrEmpty(apiName))
                {
                    apiName = DefaultApiName;
                }

                Type apiType = APIs
                    .Where(t => t.Name == apiName)
                    .FirstOrDefault();
                
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
                    response.Write(String.Format("API '{0}' not found", apiName));
                    response.End();
                }
            }
        }

        /// <summary>
        /// Returns collection of APIs types that implement IHttpHandler interface. Should be overriden in descendands.
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