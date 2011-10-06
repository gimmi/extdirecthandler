using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectHandler
	{
		private readonly ObjectFactory _objectFactory;
		private readonly IMetadata _metadata;
		private readonly ParametersParser _parametersParser;

		public DirectHandler(ObjectFactory objectFactory, IMetadata metadata)
			: this(objectFactory, metadata, new ParametersParser()) {}

		internal DirectHandler(ObjectFactory objectFactory, IMetadata metadata, ParametersParser parametersParser)
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
			MethodInfo methodInfo = _metadata.GetMethodInfo(request.Action, request.Method);
			object result;
			try
			{
				object[] parameters = _parametersParser.Parse(methodInfo.GetParameters(), request.JsonData, request.FormData, jsonSerializer);
				result = methodInfo.Invoke(actionInstance, parameters);
			}
			catch(TargetInvocationException e)
			{
				return new DirectResponse(request, e.InnerException);
			}
			catch(Exception e)
			{
				return new DirectResponse(request, e);
			}
			return new DirectResponse(request, SerializeResult(result, jsonSerializer));
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