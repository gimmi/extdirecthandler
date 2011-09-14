using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;

namespace ExtDirectHandler
{
	public class DirectHttpHandler : IHttpHandler
	{
		private static Metadata _metadata = new Metadata();
		private static ObjectFactory _objectFactory = new ObjectFactory();

		public bool IsReusable
		{
			get { return false; }
		}

		public static void SetMetadata(Metadata metadata)
		{
			_metadata = metadata;
		}

		public static void SetObjectFactory(ObjectFactory factory)
		{
			_objectFactory = factory;
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
			Dictionary<string, Stream> files = httpRequest.Files.AllKeys.ToDictionary(n => n, n => httpRequest.Files[n].InputStream);
			DirectRequest[] requests = new DirectRequestsBuilder().Build(new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding), httpRequest.Form, files);
			var responses = new DirectResponse[requests.Length];
			for(int i = 0; i < requests.Length; i++)
			{
				responses[i] = new DirectHandler(_objectFactory, _metadata).Handle(requests[i]);
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
			string ns = request.QueryString["ns"];
			string format = request.QueryString["format"];
			string url = request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.Unescaped);
			DirectApiBuilder apiBuilder = new DirectApiBuilder(_metadata);

			if (format == "json")
			{
				response.ContentType = "application/json";
				response.Write(apiBuilder.BuildApiDescriptor(ns, ns, url));
			}
			else
			{
				response.ContentType = "text/javascript";
				response.Write(apiBuilder.BuildApi(ns, url));
			}
		}
	}
}