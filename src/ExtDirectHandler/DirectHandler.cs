using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
			try
			{
				Type type = _metadata.GetActionType(request.Action);
				MethodInfo methodInfo = _metadata.GetMethodInfo(request.Action, request.Method);
				JsonSerializer jsonSerializer = _objectFactory.GetJsonSerializer();
				try
				{
					object actionInstance = _objectFactory.GetInstance(type);
					try
					{
						return Handle(request, jsonSerializer, methodInfo, actionInstance);
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
			}
			catch(Exception e)
			{
				return new DirectResponse(request, e);
			}
		}

		internal DirectResponse Handle(DirectRequest request, JsonSerializer jsonSerializer, MethodInfo methodInfo, object actionInstance)
		{
			object[] parameters = _parametersParser.Parse(methodInfo.GetParameters(), request.JsonData, request.FormData, jsonSerializer);
			object result;
			try
			{
				result = methodInfo.Invoke(actionInstance, parameters);
			}
			catch(TargetInvocationException e)
			{
				return new DirectResponse(request, e.InnerException);
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