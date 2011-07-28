using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpTestsEx;

namespace ExtDirectHandler.Tests
{
	[TestFixture]
	public class ParameterValuesParserTest
	{
		private ParametersParser _target;

		[SetUp]
		public void SetUp()
		{
			_target = new ParametersParser();
		}

		[Test]
		public void Should_parse_positional_arguments()
		{
			object[] actual = _target.ParseByPosition(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123), new JValue(true)), new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[Test]
		public void Should_throw_exception_when_positional_argument_count_does_not_match()
		{
			Executing.This(() => _target.ParseByPosition(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123)), new JsonSerializer())).Should().Throw<Exception>().And.Exception.Message.Should().Be.EqualTo("Method expect 3 parameter(s), but passed 2 parameter(s)");
		}

		[Test]
		public void Should_parse_named_arguments()
		{
			object[] actual = _target.ParseByName(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject { { "p3", new JValue(true) }, { "p2", new JValue(123) }, { "p1", new JValue("v1") } }, new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[Test]
		public void Should_throw_exception_when_named_arguments_are_missing_and_without_default_value()
		{
			Executing.This(() => _target.ParseByName(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject { { "p2", new JValue(123) }, { "p1", new JValue("v1") } }, new JsonSerializer())).Should().Throw<Exception>().And.Exception.Message.Should().Be.EqualTo("Method expect a parameter named 'p3', but it has not been found and does not have default value defined");
		}

		[Test]
		public void Should_use_parameter_default_value_when_named_arguments_are_missing_but_have_default_value()
		{
			object[] actual = _target.ParseByName(GetType().GetMethod("MethodWithDefaults").GetParameters(), new JObject(), new JsonSerializer());
			actual[0].Should().Be.EqualTo("default string");
			actual[1].Should().Be.EqualTo(123);
			actual[2].Should().Be.EqualTo(true);
		}

		[Test]
		public void Should_handle_parameters_of_jtoken_type_as_expected()
		{
			var jObject = new JObject { { "prop", new JValue("val") } };
			var jArray = new JArray(new JValue(1));
			object[] actual = _target.ParseByPosition(GetType().GetMethod("MethodWithJTokenParam").GetParameters(), new JArray(jObject, jArray), new JsonSerializer());
			actual.Length.Should().Be.EqualTo(2);
			JToken.DeepEquals((JToken)actual[0], jObject).Should().Be.True();
			JToken.DeepEquals((JToken)actual[1], jArray).Should().Be.True();
		}

		public void ExampleMethod(string p1, int p2, bool p3) {}
		public void MethodWithDefaults(string p1 = "default string", int p2 = 123, bool p3 = true) {}
		public void MethodWithJTokenParam(JObject p1, JArray p2) {}
	}
}