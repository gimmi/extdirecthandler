﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpTestsEx;

namespace ExtDirectHandler.VsTests
{
	[TestClass]
	public class ParameterValuesParserTest // TODO rename
	{
		private ParametersParser _target;

		[TestInitialize]
		public void SetUp()
		{
			_target = new ParametersParser();
		}

		[TestMethod]
		public void Should_parse_json_positional_arguments()
		{
			object[] actual = _target.ParseJsonArray(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123), new JValue(true)), new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[TestMethod]
		public void Should_throw_exception_when_json_positional_argument_count_does_not_match()
		{
			Executing.This(() => _target.ParseJsonArray(GetType().GetMethod("ExampleMethod").GetParameters(), new JArray(new JValue("v1"), new JValue(123)), new JsonSerializer())).Should().Throw<Exception>().And.Exception.Message.Should().Be.EqualTo("Method expect 3 parameter(s), but passed 2 parameter(s)");
		}

		[TestMethod]
		public void Should_parse_json_named_arguments()
		{
			object[] actual = _target.ParseJsonObject(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject { { "p3", new JValue(true) }, { "p2", new JValue(123) }, { "p1", new JValue("v1") } }, new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", 123, true });
		}

		[TestMethod]
		public void Should_use_type_default_value_when_json_named_arguments_are_missing()
		{
			object[] actual = _target.ParseJsonObject(GetType().GetMethod("ExampleMethod").GetParameters(), new JObject(), new JsonSerializer());
			actual[0].Should().Be.Null();
			actual[1].Should().Be.EqualTo(0);
			actual[2].Should().Be.EqualTo(false);
		}

		[TestMethod]
		public void Should_handle_json_parameters_of_jtoken_type_as_expected()
		{
			var jObject = new JObject { { "prop", new JValue("val") } };
			var jArray = new JArray(new JValue(1));
			object[] actual = _target.ParseJsonArray(GetType().GetMethod("MethodWithJTokenParam").GetParameters(), new JArray(jObject, jArray), new JsonSerializer());
			actual.Length.Should().Be.EqualTo(2);
			JToken.DeepEquals((JToken)actual[0], jObject).Should().Be.True();
			JToken.DeepEquals((JToken)actual[1], jArray).Should().Be.True();
		}

		[TestMethod]
		public void Should_handle_form_parameters()
		{
			var httpInputStream = TestUtils.StubHttpInputStream();
			var httpPostedFile = TestUtils.StubHttpPostedFile(httpInputStream);
			var formData = new Dictionary<string, object> {
				{ "p1", "v1" },
				{ "p2", "v2" },
				{ "p3", httpPostedFile },
				{ "p4", httpPostedFile }
			};

			object[] actual = _target.Parse(GetType().GetMethod("FormDataMethod").GetParameters(), new JArray(), formData, new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", "v2", httpInputStream, httpPostedFile });
		}

		[TestMethod]
		public void Should_use_type_default_value_when_form_arguments_are_missing()
		{
			var formData = new Dictionary<string, object> {
				{ "p1", "v1" }
			};

			object[] actual = _target.Parse(GetType().GetMethod("FormDataMethod").GetParameters(), new JArray(), formData, new JsonSerializer());
			actual.Should().Have.SameSequenceAs(new object[] { "v1", null, null, null });
		}

		public void ExampleMethod(string p1, int p2, bool p3) {}
		public void FormDataMethod(string p1, string p2, Stream p3, HttpPostedFile p4) {}
		public void MethodWithJTokenParam(JObject p1, JArray p2) {}
	}
}