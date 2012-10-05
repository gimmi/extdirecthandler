using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;

namespace ExtDirectHandler
{
	public class DirectHttpHandler : IHttpHandler, IRequiresSessionState // See http://stackoverflow.com/questions/1382791
	{
		private static IMetadata _metadata = new ReflectionConfigurator();
		private static DirectHandlerInterceptor _directHandlerInterceptor = (type, info, invoker) => invoker.Invoke();

		public bool IsReusable
		{
			get { return false; }
		}

		public static void SetMetadata(IMetadata metadata)
		{
			_metadata = metadata;
		}

		public static void SetDirectHandlerInterceptor(DirectHandlerInterceptor directHandlerInterceptor)
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
			for(int i = 0; i < requests.Length; i++)
			{
				responses[i] = new DirectHandler(_metadata, _directHandlerInterceptor).Handle(requests[i]);
			}
			using(var textWriter = new StreamWriter(httpResponse.OutputStream, httpResponse.ContentEncoding))
			{
				if(requests[0].Upload)
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
			string url = request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.Unescaped);
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