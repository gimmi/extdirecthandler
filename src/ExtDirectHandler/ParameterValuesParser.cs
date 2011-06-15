using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class ParameterValuesParser
	{
		internal object[] ParseByPosition(ParameterInfo[] parameterInfos, JToken[] data, JsonSerializer jsonSerializer)
		{
			if(parameterInfos.Length != data.Length)
			{
				throw new Exception(string.Format("Method expect {0} parameter(s), but passed {1} parameter(s)", parameterInfos.Length, data.Length));
			}
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = jsonSerializer.Deserialize(new JTokenReader(data[i]), parameterInfos[i].ParameterType);
			}
			return parameters;
		}
	}
}