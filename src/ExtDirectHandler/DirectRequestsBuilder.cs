using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectRequestsBuilder
	{
		public DirectRequest[] BuildFromRequestData(TextReader reader)
		{
			JToken jToken = JToken.Load(new JsonTextReader(reader));
			return new JsonSerializer().Deserialize<DirectRequest[]>(new JTokenReader(jToken.Type == JTokenType.Array ? jToken : new JArray(jToken)));
		}

		public DirectRequest[] BuildFromFormData(NameValueCollection form)
		{
			var request = new DirectRequest();
			var data = new JObject();
			foreach(string key in form.AllKeys)
			{
				switch(key)
				{
					case "extTID":
						request.Tid = int.Parse(form[key]);
						break;
					case "extAction":
						request.Action = form[key];
						break;
					case "extMethod":
						request.Method = form[key];
						break;
					case "extType":
						request.Type = form[key];
						break;
					case "extUpload":
						request.Upload = bool.Parse(form[key]);
						break;
					default:
						data.Add(key, new JValue(form[key]));
						break;
				}
			}
			request.Data = new JToken[] { data };
			return new[] { request };
		}
	}
}