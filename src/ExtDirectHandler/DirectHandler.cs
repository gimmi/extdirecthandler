using System;
using System.Reflection;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectHandler
	{
		private readonly ObjectFactory _objectFactory;
		private readonly Metadata _metadata;

		public DirectHandler(ObjectFactory objectFactory, Metadata metadata)
		{
			_objectFactory = objectFactory;
			_metadata = metadata;
		}

		public DirectResponse Handle(DirectRequest request)
		{
			var jsonSerializer = _objectFactory.GetJsonSerializer();
			try
			{
				return Handle(request, jsonSerializer);
			}
			finally
			{
				_objectFactory.Release(jsonSerializer);
			}
		}

		private DirectResponse Handle(DirectRequest request, JsonSerializer jsonSerializer)
		{
			object actionInstance = _objectFactory.GetInstance(_metadata.GetActionType(request.Action));
			try
			{
				return Handle(request, jsonSerializer, actionInstance);
			}
			finally
			{
				_objectFactory.Release(actionInstance);
			}
		}

		internal DirectResponse Handle(DirectRequest request, JsonSerializer jsonSerializer, object actionInstance)
		{
			var response = new DirectResponse(request);
			MethodInfo methodInfo = _metadata.GetMethodInfo(request.Action, request.Method);
			object result = null;
			try
			{
				object[] parameters = GetParameterValues(methodInfo.GetParameters(), request.Data, jsonSerializer);
				result = methodInfo.Invoke(actionInstance, parameters);
			}
			catch(TargetInvocationException e)
			{
				response.SetException(e.InnerException);
			}
			catch(Exception e)
			{
				response.SetException(e);
			}
			response.Result = SerializeResult(result, jsonSerializer);
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

		internal object[] GetParameterValues(ParameterInfo[] parameterInfos, JToken[] data, JsonSerializer jsonSerializer)
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