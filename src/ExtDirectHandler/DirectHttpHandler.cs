using System;
using System.IO;
using System.Web;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;

namespace ExtDirectHandler
{
	public class DirectHttpHandler : IHttpHandler
	{
		private static Metadata _metadata;
		private static ObjectFactory _objectFactory;
		private string _namespace;

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

		public void SetNamespace(string ns)
		{
			_namespace = ns;
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
			response.ContentType = "text/javascript";
			response.Write("Ext.ns('Ext.app');");
			response.Write(Environment.NewLine);
			response.Write("Ext.app.REMOTING_API = ");
			SerializeResponse(response, new DirectApiBuilder(_metadata).BuildApi(_namespace, request.Url.ToString()));
			response.Write(";");
		}

		private DirectRequest DeserializeRequest(HttpRequest request)
		{
			var sr = new StreamReader(request.InputStream, request.ContentEncoding);
			using(JsonReader jsonReader = new JsonTextReader(sr))
			{
				return new JsonSerializer().Deserialize<DirectRequest>(jsonReader);
			}
		}

		private void SerializeResponse(HttpResponse response, object value)
		{
			using(var jsonWriter = new JsonTextWriter(new StreamWriter(response.OutputStream, response.ContentEncoding)))
			{
				jsonWriter.Formatting = Formatting.Indented;
				jsonWriter.Indentation = 1;
				jsonWriter.IndentChar = '\t';
				new JsonSerializer().Serialize(jsonWriter, value);
			}
		}
	}
}