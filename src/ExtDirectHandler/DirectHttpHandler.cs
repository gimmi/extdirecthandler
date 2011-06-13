using System;
using System.IO;
using System.Web;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

		public static void SetMetadata(Metadata metadata)
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

		private void DoPost(HttpRequest httpRequest, HttpResponse httpResponse)
		{
			DirectRequest[] requests;
			if (httpRequest.Form.Count > 0)
			{
				var request = new DirectRequest();
				var data = new JObject();
				foreach(string key in httpRequest.Form.AllKeys)
				{
					switch(key)
					{
						case "extTID":
							request.Tid = int.Parse(httpRequest.Params[key]);
							break;
						case "extAction":
							request.Action = httpRequest.Params[key];
							break;
						case "extMethod":
							request.Method = httpRequest.Params[key];
							break;
						case "extType":
							request.Type = httpRequest.Params[key];
							break;
						case "extUpload":
							request.Upload = bool.Parse(httpRequest.Params[key]);
							break;
						default:
							data.Add(key, new JValue(httpRequest.Params[key]));
							break;
					}
				}
				request.Data = new JToken[] { data };
				requests = new[] { request };
			}
			else
			{
				JToken jToken = JToken.Load(new JsonTextReader(new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding)));
				requests = new JsonSerializer().Deserialize<DirectRequest[]>(new JTokenReader(jToken.Type == JTokenType.Array ? jToken : new JArray(jToken)));
			}
			var responses = new DirectResponse[requests.Length];
			for(int i = 0; i < requests.Length; i++)
			{
				responses[i] = new DirectHandler(_objectFactory, _metadata).Handle(requests[i]);
			}
			using(var jsonWriter = new JsonTextWriter(new StreamWriter(httpResponse.OutputStream, httpResponse.ContentEncoding)))
			{
				new JsonSerializer().Serialize(jsonWriter, responses.Length == 1 ? (object)responses[0] : responses);
			}
		}

		private void DoGet(HttpRequest request, HttpResponse response)
		{
			string ns = request.QueryString["ns"];
			response.ContentType = "text/javascript";
			string url = request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.Unescaped);
			response.Write(new DirectApiBuilder(_metadata).BuildApi(ns, url));
		}
	}
}