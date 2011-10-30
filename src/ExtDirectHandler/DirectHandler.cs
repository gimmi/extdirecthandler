using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ExtDirectHandler
{
	internal class DirectHandler
	{
		private readonly IMetadata _metadata;
		private readonly ParametersParser _parametersParser;
		private readonly DirectHandlerInterceptor _directHandlerInterceptor;

		public DirectHandler(IMetadata metadata, DirectHandlerInterceptor directHandlerInterceptor)
			: this(metadata, new ParametersParser(), directHandlerInterceptor) {}

		internal DirectHandler(IMetadata metadata, ParametersParser parametersParser, DirectHandlerInterceptor directHandlerInterceptor)
		{
			_metadata = metadata;
			_parametersParser = parametersParser;
			_directHandlerInterceptor = directHandlerInterceptor;
		}

		public DirectResponse Handle(DirectRequest request)
		{
			try
			{
				Type type = _metadata.GetActionType(request.Action, request.Method);
				MethodInfo methodInfo = _metadata.GetMethodInfo(request.Action, request.Method);

				var response = new DirectResponse(request);
				_directHandlerInterceptor.Invoke(type, methodInfo, delegate(object actionInstance, JsonSerializer jsonSerializer) {
					response = Handle(request, jsonSerializer, methodInfo, actionInstance, type);
				});
				return response;

				//JsonSerializer jsonSerializer = _objectFactory.GetJsonSerializer();
				//try
				//{
				//    object actionInstance = _objectFactory.GetInstance(type);
				//    try
				//    {
				//        return Handle(request, jsonSerializer, methodInfo, actionInstance);
				//    }
				//    finally
				//    {
				//        _objectFactory.Release(actionInstance);
				//    }
				//}
				//finally
				//{
				//    _objectFactory.Release(jsonSerializer);
				//}
			}
			catch(TargetInvocationException e)
			{
				return new DirectResponse(request, e.InnerException);
			}
			catch(Exception e)
			{
				return new DirectResponse(request, e);
			}
		}

		internal DirectResponse Handle(DirectRequest request, JsonSerializer jsonSerializer, MethodInfo methodInfo, object actionInstance, Type type)
		{
			actionInstance = actionInstance ?? Activator.CreateInstance(type);
			jsonSerializer = jsonSerializer ?? new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			object[] parameters = _parametersParser.Parse(methodInfo.GetParameters(), request.JsonData, request.FormData, jsonSerializer);
			object result = methodInfo.Invoke(actionInstance, parameters);
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