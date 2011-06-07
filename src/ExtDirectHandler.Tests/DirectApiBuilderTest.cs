using System.Collections.Generic;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using SharpTestsEx;

namespace ExtDirectHandler.Tests
{
	[TestFixture]
	public class DirectApiBuilderTest
	{
		private List<DirectActionMetadata> _actions;
		private DirectApiBuilder _target;

		[SetUp]
		public void SetUp()
		{
			_actions = new List<DirectActionMetadata>();
			_target = new DirectApiBuilder(_actions);
		}

		[Test]
		public void Should_build_api()
		{
			_actions.Add(new DirectActionMetadata{
				Name = "Action1",
				Methods = new Dictionary<string, DirectMethodMetadata>{
					{ "method1", new DirectMethodMetadata{ Name = "method1" } },
					{ "method2", new DirectMethodMetadata{ Name = "method2" } }
				}
			});
			var actual = _target.BuildApi("namespace", "url").ToString().Replace('"', '\'');
			actual.Should().Be.EqualTo("");
		}

		private class ActionClass1
		{
			public void Method1(string p1, string p2) {}
			public void Method2(string p1) {}
		}
	}
}