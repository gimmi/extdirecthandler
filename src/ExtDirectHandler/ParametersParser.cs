using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	public class ParametersParser
	{
		public virtual object[] Parse(ParameterInfo[] parameterInfos, JToken jsonData, IDictionary<string, object> formData, JsonSerializer jsonSerializer)
		{
			if(formData.Count > 0)
			{
				return ParseForm(parameterInfos, formData);
			}
			if(jsonData.Type == JTokenType.Array)
			{
				return ParseJsonArray(parameterInfos, (JArray)jsonData, jsonSerializer);
			}
			if(jsonData.Type == JTokenType.Object)
			{
				return ParseJsonObject(parameterInfos, (JObject)jsonData, jsonSerializer);
			}
			throw new Exception("Cannot parse parameters");
		}

		public virtual object[] ParseJsonArray(ParameterInfo[] parameterInfos, JArray json, JsonSerializer jsonSerializer)
		{
			if(parameterInfos.Length != json.Count)
			{
				throw new Exception(string.Format("Method expect {0} parameter(s), but passed {1} parameter(s)", parameterInfos.Length, json.Count));
			}
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = Deserialize(parameterInfos[i], json[i], jsonSerializer);
			}
			return parameters;
		}

		public virtual object[] ParseJsonObject(ParameterInfo[] parameterInfos, JObject json, JsonSerializer jsonSerializer)
		{
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				JToken value;
				if(json.TryGetValue(parameterInfos[i].Name, out value))
				{
					parameters[i] = Deserialize(parameterInfos[i], value, jsonSerializer);
				}
				else
				{
					parameters[i] = GetDefaultValue(parameterInfos[i].ParameterType);
				}
			}
			return parameters;
		}

		public virtual object[] ParseForm(ParameterInfo[] parameterInfos, IDictionary<string, object> form)
		{
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				object value;
				if(form.TryGetValue(parameterInfos[i].Name, out value))
				{
					if (value is HttpPostedFile && parameterInfos[i].ParameterType.IsAssignableFrom(typeof(Stream)))
					{
						value = ((HttpPostedFile)value).InputStream;
					}
					parameters[i] = value;
				}
				else
				{
					parameters[i] = GetDefaultValue(parameterInfos[i].ParameterType);
				}
			}
			return parameters;
		}

		private static object GetDefaultValue(Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

		private static object Deserialize(ParameterInfo parameterInfo, JToken value, JsonSerializer jsonSerializer)
		{
			return jsonSerializer.Deserialize(new JTokenReader(value), parameterInfo.ParameterType);
		}
	}
}