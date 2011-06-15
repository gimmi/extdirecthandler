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
		private ParameterValuesParser _target;

		[SetUp]
		public void SetUp()
		{
			_target = new ParameterValuesParser();
		}

		[Test]
		public void Should_parse_based_on_argument_position()
		{
			object[] actual = _target.ParseByPosition(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123), new JValue(true)), new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[Test]
		public void Should_throw_exception_when_argument_count_does_not_match()
		{
			Executing.This(() => _target.ParseByPosition(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123)), new JsonSerializer())).Should().Throw<Exception>().And.Exception.Message.Should().Be.EqualTo("Method expect 3 parameter(s), but passed 2 parameter(s)");
		}

		[Test]
		public void Should_parse_based_on_argument_name()
		{
			object[] actual = _target.ParseByName(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject { { "p3", new JValue(true) }, { "p2", new JValue(123) }, { "p1", new JValue("v1") } }, new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[Test]
		public void Should_throw_exception_when_some_arguments_are_missing()
		{
			Executing.This(() => _target.ParseByName(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject { { "p2", new JValue(123) }, { "p1", new JValue("v1") } }, new JsonSerializer())).Should().Throw<Exception>().And.Exception.Message.Should().Be.EqualTo("Method expect a parameter named 'p3', but it has not been found");
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
		public void MethodWithJTokenParam(JObject p1, JArray p2) {}
	}
}