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

			_metadata.Stub(x => x.GetActionType("Action")).Return(typeof(Action));
			_metadata.Stub(x => x.GetMethodInfo("Action", "method")).Return(typeof(Action).GetMethod("Method"));

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
			_metadata.Stub(x => x.GetActionType("Action")).Return(typeof(Action));
			_metadata.Stub(x => x.GetMethodInfo("Action", "method")).Return(typeof(Action).GetMethod("Method"));

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
		public void Should_invoke_expected_method_passing_parameters_and_returning_result()
		{
			var actionInstance = MockRepository.GenerateMock<Action>();
			actionInstance.Expect(x => x.MethodWithParams(123, "str", true)).Return("ret");

			_metadata.Stub(x => x.GetActionType("Action")).Return(typeof(Action));
			_metadata.Stub(x => x.GetMethodInfo("Action", "methodWithParams")).Return(typeof(Action).GetMethod("MethodWithParams"));

			DirectResponse response = _target.Handle(new DirectRequest{
				Action = "Action",
				Method = "methodWithParams",
				Data = new JToken[]{ new JValue(123), new JValue("str"), new JValue(true) }
			}, new JsonSerializer(), actionInstance);

			((object)response.Result).Should().Be.EqualTo(new JValue("ret"));

			actionInstance.VerifyAllExpectations();
		}

		public class Action
		{
			public virtual void Method() {}

			public virtual string MethodWithParams(int intValue, string stringValue, bool boolValue)
			{
				return null;
			}
		}
	}
}