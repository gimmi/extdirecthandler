using System;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectRequestsBuilder
	{
		public DirectRequest[] Build(TextReader content, NameValueCollection formData)
		{
			return formData.Count > 0 ? BuildFromFormData(formData) : BuildFromContent(content);
		}

		public DirectRequest[] BuildFromContent(TextReader content)
		{
			JToken json = JToken.Load(new JsonTextReader(content));
			return new JsonSerializer().Deserialize<DirectRequest[]>(new JTokenReader(json.Type == JTokenType.Array ? json : new JArray(json)));
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
						request.Tid = Int32.Parse(form[key]);
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
						request.Upload = Boolean.Parse(form[key]);
						break;
					default:
						data.Add(key, new JValue(form[key]));
						break;
				}
			}
			request.JsonData = data;
			return new[] { request };
		}
	}
}