using System;
using System.IO;
using System.Web;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExtDirectHandler
{
	public class DirectHttpHandler : IHttpHandler
	{
		private static Metadata _metadata;
		private static ObjectFactory _objectFactory = new ObjectFactory();

		public bool IsReusable
		{
			get { return false; }
		}

		internal static void SetActionMetadatas(Metadata metadata)
		{
			if(_metadata != null)
			{
				throw new Exception("Already configured");
			}
			_metadata = metadata;
		}

		public static void SetObjectFactory(ObjectFactory factory)
		{
			if(_objectFactory != null)
			{
				throw new Exception("Already configured");
			}
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

		private void DoPost(HttpRequest request, HttpResponse response)
		{
			var directHandler = new DirectHandler(_objectFactory, _metadata);
			DirectRequest directRequest = DeserializeRequest(request);
			DirectResponse directResponse = directHandler.Handle(directRequest);
			SerializeResponse(response, directResponse);
		}

		private void DoGet(HttpRequest request, HttpResponse response)
		{
			string ns = request.QueryString["ns"];
			response.ContentType = "text/javascript";
			string url = request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.Unescaped);
			response.Write(new DirectApiBuilder(_metadata).BuildApi(ns, url));
		}

		private DirectRequest DeserializeRequest(HttpRequest request)
		{
			var sr = new StreamReader(request.InputStream, request.ContentEncoding);
			using(JsonReader jsonReader = new JsonTextReader(sr))
			{
				return new JsonSerializer().Deserialize<DirectRequest>(jsonReader);
			}
		}

		private void SerializeResponse(HttpResponse httpResponse, DirectResponse directResponse)
		{
			using(var jsonWriter = new JsonTextWriter(new StreamWriter(httpResponse.OutputStream, httpResponse.ContentEncoding)))
			{
				new JsonSerializer().Serialize(jsonWriter, directResponse);
			}
		}
	}
}