using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;

namespace ExtDirectHandler
{
	public class DirectHttpHandler : IHttpHandler, IRequiresSessionState // See http://stackoverflow.com/questions/1382791
	{
		private IMetadata _metadata;
		private DirectHandlerInterceptor _directHandlerInterceptor;

		public bool IsReusable
		{
			get { return false; }
		}

		public DirectHttpHandler(IMetadata metadata, DirectHandlerInterceptor directHandlerInterceptor)
		{
			_metadata = metadata ?? GetMetadata();
			_directHandlerInterceptor = directHandlerInterceptor ?? GetDirectHandlerInterceptor();
		}

		public DirectHttpHandler(IMetadata metadata)
			: this(metadata, null)
		{

		}

		public DirectHttpHandler(DirectHandlerInterceptor directHandlerInterceptor)
			: this(null, directHandlerInterceptor)
		{

		}

		public DirectHttpHandler()
			: this(null, null)
		{

		}

		protected virtual IMetadata GetMetadata()
		{
			return new ReflectionConfigurator();
		}

		protected virtual DirectHandlerInterceptor GetDirectHandlerInterceptor()
		{
			return (type, info, invoker) => invoker.Invoke();
		}

		public void SetDirectHandlerInterceptor(DirectHandlerInterceptor directHandlerInterceptor)
		{
			_directHandlerInterceptor = directHandlerInterceptor;
		}

		public void ProcessRequest(HttpContext context)
		{
			switch(context.Request.HttpMethod)
			{
				case "GET":
					DoGet(context.Request, context.Response);
					break;
				case "POST":
					DoPost(context.Request, context.Response);
					break;
			}
		}

		private void DoPost(HttpRequest httpRequest, HttpResponse httpResponse)
		{
			Dictionary<string, HttpPostedFile> files = httpRequest.Files.AllKeys.ToDictionary(n => n, n => httpRequest.Files[n]);
			DirectRequest[] requests = new DirectRequestsBuilder().Build(new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding), httpRequest.Form, files);
			var responses = new DirectResponse[requests.Length];

			var tasks = new Task[requests.Length];
			for (int i = 0; i < requests.Length; i++)
			{
				tasks[i] = new Task((state) => 
				{
					responses[(int)state] = new DirectHandler(_metadata, _directHandlerInterceptor).Handle(requests[(int)state]);
				}, i);
			}

			if (_metadata.GetAllowParallel())
			{
				tasks.ToList().ForEach(t => t.Start());
				Task.WaitAll(tasks);
			}
			else
			{
				tasks.ToList().ForEach(t => t.RunSynchronously());
			}

			WriteOutput(httpResponse, requests, responses);
		}

		private static void WriteOutput(HttpResponse httpResponse, DirectRequest[] requests, DirectResponse[] responses)
		{
			using (var textWriter = new StreamWriter(httpResponse.OutputStream, httpResponse.ContentEncoding))
			{
				if (requests[0].Upload)
				{
					httpResponse.ContentType = "text/html";
					textWriter.Write("<html><body><textarea>");
					SerializeResponse(responses, textWriter);
					textWriter.Write("</textarea></body></html>");
				}
				else
				{
					httpResponse.ContentType = "application/json";
					SerializeResponse(responses, textWriter);
				}
			}
		}

		private static void SerializeResponse(DirectResponse[] responses, TextWriter textWriter)
		{
			new JsonSerializer().Serialize(textWriter, responses.Length == 1 ? (object)responses[0] : responses);
		}

		private void DoGet(HttpRequest request, HttpResponse response)
		{
			string format = request.QueryString["format"];
			string url = request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path | UriComponents.Query, UriFormat.Unescaped);
			var apiBuilder = new DirectApiBuilder(_metadata);

			if(string.Equals(format, "json", StringComparison.InvariantCultureIgnoreCase))
			{
				response.ContentType = "application/json";
				response.Write(apiBuilder.BuildJson(url));
			}
			else
			{
				response.ContentType = "text/javascript";
				response.Write(apiBuilder.BuildJavascript(url));
			}
		}
	}
}