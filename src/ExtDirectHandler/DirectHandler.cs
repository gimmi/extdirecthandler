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
		private readonly ParametersParser _parametersParser;

		public DirectHandler(ObjectFactory objectFactory, Metadata metadata)
			: this(objectFactory, metadata, new ParametersParser()) {}

		internal DirectHandler(ObjectFactory objectFactory, Metadata metadata, ParametersParser parametersParser)
		{
			_objectFactory = objectFactory;
			_metadata = metadata;
			_parametersParser = parametersParser;
		}

		public DirectResponse Handle(DirectRequest request)
		{
			JsonSerializer jsonSerializer = _objectFactory.GetJsonSerializer();
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
				object[] parameters = _parametersParser.Parse(methodInfo.GetParameters(), request.JsonData, jsonSerializer);
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
	}
}