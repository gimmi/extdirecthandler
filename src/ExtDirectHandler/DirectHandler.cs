using System.Collections.Generic;
using System.Reflection;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectHandler
	{
		private readonly ObjectFactory _objectFactory;
		private readonly IDictionary<string, DirectActionMetadata> _actionMetadatas;

		public DirectHandler(ObjectFactory objectFactory, IDictionary<string, DirectActionMetadata> actionMetadatas)
		{
			_objectFactory = objectFactory;
			_actionMetadatas = actionMetadatas;
		}

		public DirectResponse Handle(DirectRequest request)
		{
			var response = new DirectResponse(request);
			var jsonSerializer = (JsonSerializer)_objectFactory.GetInstance(typeof(JsonSerializer));
			try
			{
				DirectActionMetadata directActionMetadata = _actionMetadatas[request.Action];
				MethodInfo methodInfo = directActionMetadata.Methods[request.Method];
				object[] parameters = GetParameterValues(methodInfo.GetParameters(), request.Data, jsonSerializer);
				object actionInstance = _objectFactory.GetInstance(directActionMetadata.Type);
				try
				{
					response.Result = SerializeResult(methodInfo.Invoke(actionInstance, parameters), jsonSerializer);
				}
				finally
				{
					_objectFactory.Release(actionInstance);
				}
			}
			finally
			{
				_objectFactory.Release(jsonSerializer);
			}
			return response;
		}

		private JToken SerializeResult(object result, JsonSerializer jsonSerializer)
		{
			using(var writer = new JTokenWriter())
			{
				jsonSerializer.Serialize(writer, result);
				return writer.Token;
			}
		}

		private object[] GetParameterValues(ParameterInfo[] parameterInfos, JToken[] data, JsonSerializer jsonSerializer)
		{
			var parameters = new object[parameterInfos.Length];
			for(int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = jsonSerializer.Deserialize(new JTokenReader(data[i]), parameterInfos[i].ParameterType);
			}
			return parameters;
		}
	}
}