using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using SpikeHttpHandler.Configuration;

namespace SpikeHttpHandler
{
	public class DirectHttpHandler : IHttpHandler
	{
		private static IDictionary<string, DirectActionMetadata> _actionMetadatas;
		private static DirectActionFactory _actionFactory;
		private readonly JsonSerializer _serializer = new JsonSerializer();

		public static void SetActionMetadatas(IDictionary<string, DirectActionMetadata> actions)
		{
			if (_actionMetadatas != null)
			{
				throw new Exception("Already configured");
			}
			_actionMetadatas = actions;
		}

		public static void SetActionFactory(DirectActionFactory factory)
		{
			if (_actionFactory != null)
			{
				throw new Exception("Already configured");
			}
			_actionFactory = factory;
		}

		public static DirectActionFactory ActionFactory
		{
			set { _actionFactory = value; }
		}

		public JsonSerializer Serializer
		{
			get { return _serializer; }
		}

		public bool IsReusable
		{
			get { return false; }
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
			SerializeResponse(response, new DirectHandler(_actionFactory).handle(DeserializeRequest(request)));
		}

		private DirectRequest DeserializeRequest(HttpRequest request)
		{
			var sr = new StreamReader(request.InputStream, request.ContentEncoding);
			using(JsonReader jsonReader = new JsonTextReader(sr))
			{
				return _serializer.Deserialize<DirectRequest>(jsonReader);
			}
		}

		private void SerializeResponse(HttpResponse response, DirectResponse value)
		{
			using(var jsonWriter = new JsonTextWriter(new StreamWriter(response.OutputStream, response.ContentEncoding)))
			{
				_serializer.Serialize(jsonWriter, value);
			}
		}

		private void DoGet(HttpRequest request, HttpResponse response)
		{
			response.Write("Hello from custom handler.");
		}
	}
}