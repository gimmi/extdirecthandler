using System;
using ExtDirectHandler.Configuration;
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
		private ObjectFactory _objectFactory;
		private DirectHandler _target;
		private Metadata _metadata;

		[SetUp]
		public void SetUp()
		{
			_objectFactory = MockRepository.GenerateStub<ObjectFactory>();
			_metadata = MockRepository.GenerateStub<Metadata>();
			_target = new DirectHandler(_objectFactory, _metadata);

			_metadata.Stub(x => x.GetActionType("Action")).Return(typeof(Action));
			_metadata.Stub(x => x.GetMethodInfo("Action", "method")).Return(typeof(Action).GetMethod("Method"));
			_metadata.Stub(x => x.GetMethodInfo("Action", "methodWithParams")).Return(typeof(Action).GetMethod("MethodWithParams"));
			_metadata.Stub(x => x.GetMethodInfo("Action", "methodThatThrowException")).Return(typeof(Action).GetMethod("MethodThatThrowException"));
			_metadata.Stub(x => x.GetMethodInfo("Action", "methodWithRawParameters")).Return(typeof(Action).GetMethod("MethodWithRawParameters"));
		}

		[Test]
		public void Should_use_objectfactory_to_obtain_and_release_object_instances()
		{
			var actionInstance = MockRepository.GenerateStub<Action>();
			_objectFactory.Expect(x => x.GetInstance(typeof(Action))).Return(actionInstance);
			_objectFactory.Expect(x => x.Release(actionInstance));

			var jsonSerializer = new JsonSerializer();
			_objectFactory.Expect(x => x.GetInstance(typeof(JsonSerializer))).Return(jsonSerializer);
			_objectFactory.Expect(x => x.Release(jsonSerializer));

			_target.Handle(new DirectRequest{
				Action = "Action",
				Method = "method",
				Data = new JToken[0]
			});

			_objectFactory.VerifyAllExpectations();
		}

		[Test]
		public void Should_build_response_based_on_request_data()
		{
			DirectResponse actual = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "method",
				Data = new JToken[0],
				Tid = 123,
				Type = "rpc"
			}, new JsonSerializer(), new Action());

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
			DirectResponse actual = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "methodThatThrowException",
				Data = new JToken[0],
				Tid = 123,
				Type = "rpc"
			}, new JsonSerializer(), new Action());

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
			var actionInstance = MockRepository.GenerateMock<Action>();
			actionInstance.Expect(x => x.MethodWithParams(123, "str", true)).Return("ret");

			DirectResponse response = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "methodWithParams",
				Data = new JToken[]{ new JValue(123), new JValue("str"), new JValue(true) }
			}, new JsonSerializer(), actionInstance);

			response.Result.ToString().Should().Be.EqualTo("ret");

			actionInstance.VerifyAllExpectations();
		}

		[Test]
		public void Should_parse_jtoken_values_as_expected()
		{
			var actionInstance = MockRepository.GenerateMock<Action>();
			actionInstance.Expect(x => x.MethodWithRawParameters(Arg<JToken>.Matches(y => y.ToString() == "value"))).Return(new JValue("ret"));

			DirectResponse response = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "methodWithRawParameters",
				Data = new JToken[]{ new JValue("value") }
			}, new JsonSerializer(), actionInstance);

			response.Result.ToString().Should().Be.EqualTo("ret");
		}

		[Test]
		public void Should_return_error_when_passed_wrong_number_of_parameters()
		{
			var actionInstance = MockRepository.GenerateMock<Action>();
			actionInstance.Expect(x => x.MethodWithRawParameters(Arg<JToken>.Matches(y => y.ToString() == "value"))).Return(new JValue("ret"));

			DirectResponse response = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "method",
				Data = new JToken[]{ new JValue("value") }
			}, new JsonSerializer(), actionInstance);

			response.Result.Should().Be.Null();
			response.Type.Should().Be.EqualTo("exception");
			response.Message.Should().Be.EqualTo("Method expect 0 parameter(s), but passed 1 parameter(s)");
			response.Where.Should().Contain("Method expect 0 parameter(s), but passed 1 parameter(s)");
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