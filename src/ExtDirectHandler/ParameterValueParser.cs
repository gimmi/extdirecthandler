using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	public class ParameterValueParser
	{
		public virtual object[] ParseByPosition(ParameterInfo[] parameterInfos, JArray data, JsonSerializer jsonSerializer)
		{
			if(parameterInfos.Length != data.Count)
			{
				throw new Exception(string.Format("Method expect {0} parameter(s), but passed {1} parameter(s)", parameterInfos.Length, data.Count));
			}
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = Deserialize(parameterInfos[i], data[i], jsonSerializer);
			}
			return parameters;
		}

		public virtual object[] ParseByName(ParameterInfo[] parameterInfos, JObject data, JsonSerializer jsonSerializer)
		{
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				JToken value;
				if(data.TryGetValue(parameterInfos[i].Name, out value))
				{
					parameters[i] = Deserialize(parameterInfos[i], value, jsonSerializer);
				}
				else
				{
					throw new Exception(string.Format("Method expect a parameter named '{0}', but it has not been found", parameterInfos[i].Name));
				}
			}
			return parameters;
		}

		private static object Deserialize(ParameterInfo parameterInfo, JToken value, JsonSerializer jsonSerializer)
		{
			return jsonSerializer.Deserialize(new JTokenReader(value), parameterInfo.ParameterType);
		}
	}
}