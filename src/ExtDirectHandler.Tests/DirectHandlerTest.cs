using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rhino.Mocks;
using SharpTestsEx;

namespace ExtDirectHandler.Tests
{
	[TestFixture]
	public class DirectHandlerTest
	{
		[Test]
		public void Should_call_interceptor_with_expected_parameters()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var directHandlerInterceptor = MockRepository.GenerateMock<DirectHandlerInterceptor>();
			var target = new DirectHandler(metadata, parametersParser, directHandlerInterceptor);

			directHandlerInterceptor.Expect(x => x.Invoke(Arg<Type>.Is.Same(typeof(Action)), Arg<MethodInfo>.Is.Same(typeof(Action).GetMethod("Method")), Arg<DirectHandlerInvoker>.Is.Anything));

			target.Handle(new DirectRequest {
				Action = "Action",
				Method = "method",
				JsonData = new JArray()
			});

			directHandlerInterceptor.VerifyAllExpectations();
		}

		[Test]
		public void Should_return_null_result_if_interceptor_does_not_invoke_handler()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, delegate { });

			DirectResponse actual = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "methodThatThrowException",
				JsonData = new JArray(),
				Tid = 123,
				Type = "rpc"
			});

			actual.Type.Should().Be.EqualTo("rpc");
			actual.Result.Should().Be.Null();
		}

		[Test]
		public void Should_return_error_if_interceptor_throws_exceptions()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, delegate {
				throw new Exception("error from interceptor");
			});

			DirectResponse actual = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "method",
				JsonData = new JArray(),
				Tid = 123,
				Type = "rpc"
			});

			actual.Type.Should().Be.EqualTo("exception");
			actual.Message.Should().Be.EqualTo("error from interceptor");
			actual.Where.Should().Contain("error from interceptor");
		}

		[Test]
		public void Should_use_actioninstance_from_interceptor()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			var actionInstance = MockRepository.GenerateMock<Action>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, delegate(Type type, MethodInfo method, DirectHandlerInvoker invoker) {
				invoker.Invoke(actionInstance);
			});

			actionInstance.Expect(x => x.Method());

			target.Handle(new DirectRequest {
				Action = "Action",
				Method = "method",
				JsonData = new JArray(),
				Tid = 123,
				Type = "rpc"
			});

			actionInstance.VerifyAllExpectations();
		}

		[Test]
		public void Should_build_response_based_on_request_data()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, (type, info, invoker) => invoker.Invoke());

			DirectResponse actual = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "method",
				JsonData = new JArray(),
				Tid = 123,
				Type = "rpc"
			});

			actual.Action.Should().Be.EqualTo("Action");
			actual.Method.Should().Be.EqualTo("method");
			actual.Tid.Should().Be.EqualTo(123);
			actual.Type.Should().Be.EqualTo("rpc");
			actual.Message.Should().Be.Null();
			actual.Where.Should().Be.Null();
			actual.Result.Should().Be.Null();
		}

		[Test]
		public void Should_build_expected_response_when_target_method_throws_exception()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, (type, method, invoker) => invoker.Invoke());

			DirectResponse actual = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "methodThatThrowException",
				JsonData = new JArray(),
				Tid = 123,
				Type = "rpc"
			});

			actual.Action.Should().Be.EqualTo("Action");
			actual.Method.Should().Be.EqualTo("methodThatThrowException");
			actual.Tid.Should().Be.EqualTo(123);
			actual.Type.Should().Be.EqualTo("exception");
			actual.Message.Should().Be.EqualTo("something wrong happened");
			actual.Where.Should().Contain("something wrong happened");
			actual.Result.Should().Be.Null();
		}

		[Test]
		public void Should_invoke_expected_method_passing_parameters_and_returning_result()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var actionInstance = MockRepository.GenerateMock<Action>();
			var target = new DirectHandler(metadata, parametersParser, (type, method, invoker) => invoker.Invoke(actionInstance));

			parametersParser.Stub(x => x.Parse(Arg<ParameterInfo[]>.Is.Anything, Arg<JToken>.Is.Anything, Arg<IDictionary<string, object>>.Is.Anything, Arg<JsonSerializer>.Is.Anything)).Return(new object[] { 123, "str", true });
			actionInstance.Expect(x => x.MethodWithParams(123, "str", true)).Return("ret");

			DirectResponse response = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "methodWithParams",
				JsonData = new JArray(new JValue(123), new JValue("str"), new JValue(true))
			});

			response.Result.ToString().Should().Be.EqualTo("ret");

			actionInstance.VerifyAllExpectations();
		}

		[Test]
		public void Should_return_error_when_fail_to_parse_parameters()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, (type, info, invoker) => invoker.Invoke());

			var actionInstance = MockRepository.GenerateMock<Action>();
			actionInstance.Expect(x => x.MethodWithRawParameters(Arg<JToken>.Matches(y => y.ToString() == "value"))).Return(new JValue("ret"));
			parametersParser.Stub(x => x.Parse(Arg<ParameterInfo[]>.Is.Anything, Arg<JToken>.Is.Anything, Arg<IDictionary<string, object>>.Is.Anything, Arg<JsonSerializer>.Is.Anything)).Throw(new Exception("stubexc"));

			DirectResponse response = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "method",
				JsonData = new JArray()
			});

			response.Result.Should().Be.Null();
			response.Type.Should().Be.EqualTo("exception");
			response.Message.Should().Be.EqualTo("stubexc");
			response.Where.Should().Contain("stubexc");
		}

		[Test]
		public void Should_return_error_when_fail_to_get_action_type()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, (type, info, invoker) => invoker.Invoke());

			metadata.Stub(x => x.GetActionType("NonExistentAction", "method")).Throw(new Exception("Action not found"));

			DirectResponse response = target.Handle(new DirectRequest {
				Action = "NonExistentAction",
				Method = "method",
				JsonData = new JArray()
			});

			response.Result.Should().Be.Null();
			response.Type.Should().Be.EqualTo("exception");
			response.Message.Should().Be.EqualTo("Action not found");
			response.Where.Should().Contain("Action not found");
		}

		[Test]
		public void Should_return_error_when_fail_to_get_methodinfo()
		{
			var parametersParser = MockRepository.GenerateStub<ParametersParser>();
			IMetadata metadata = BuildMockMetadata();
			var target = new DirectHandler(metadata, parametersParser, (type, info, invoker) => invoker.Invoke());

			metadata.Stub(x => x.GetMethodInfo("Action", "nonExistentMethod")).Throw(new Exception("Method not found"));

			DirectResponse response = target.Handle(new DirectRequest {
				Action = "Action",
				Method = "nonExistentMethod",
				JsonData = new JArray()
			});

			response.Result.Should().Be.Null();
			response.Type.Should().Be.EqualTo("exception");
			response.Message.Should().Be.EqualTo("Method not found");
			response.Where.Should().Contain("Method not found");
		}

		private IMetadata BuildMockMetadata()
		{
			var ret = MockRepository.GenerateStub<IMetadata>();
			ret.Stub(x => x.GetMethodInfo("Action", "method")).Return(typeof(Action).GetMethod("Method"));
			ret.Stub(x => x.GetMethodInfo("Action", "methodWithParams")).Return(typeof(Action).GetMethod("MethodWithParams"));
			ret.Stub(x => x.GetMethodInfo("Action", "methodThatThrowException")).Return(typeof(Action).GetMethod("MethodThatThrowException"));
			ret.Stub(x => x.GetMethodInfo("Action", "methodWithRawParameters")).Return(typeof(Action).GetMethod("MethodWithRawParameters"));
			return ret;
		}

		public class Action
		{
			public virtual void Method() {}

			public virtual void MethodThatThrowException()
			{
				throw new Exception("something wrong happened");
			}

			public virtual string MethodWithParams(int intValue, string stringValue, bool boolValue)
			{
				return null;
			}

			public virtual JToken MethodWithRawParameters(JToken par)
			{
				return par;
			}
		}
	}
}