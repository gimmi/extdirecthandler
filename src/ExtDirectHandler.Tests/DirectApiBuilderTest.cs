using System.Collections.Generic;
using System.Reflection;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using SharpTestsEx;

namespace ExtDirectHandler.Tests
{
	[TestFixture]
	public class DirectApiBuilderTest
	{
		private IDictionary<string, DirectActionMetadata> _actions;
		private DirectApiBuilder _target;

		[SetUp]
		public void SetUp()
		{
			_actions = new Dictionary<string, DirectActionMetadata>();
			_target = new DirectApiBuilder(_actions);
		}

		[Test]
		public void Should_build_api()
		{
			_actions.Add("Action1", new DirectActionMetadata{
				Methods = new Dictionary<string, MethodInfo>{
					{ "method1", typeof(ActionClass1).GetMethod("Method1") },
					{ "method2", typeof(ActionClass1).GetMethod("Method2") }
				}
			});
			_actions.Add("Action2", new DirectActionMetadata{
				Methods = new Dictionary<string, MethodInfo>{
					{ "method1", typeof(ActionClass2).GetMethod("Method1") },
				}
			});
			var actual = _target.BuildApi("namespace", "url").ToString().Replace('"', '\'');

			actual.Should().Be.EqualTo(@"{
  'id': 'namespace',
  'url': 'url',
  'type': 'remoting',
  'namespace': 'namespace',
  'actions': {
    'Action1': {
      'method1': {
        'name': 'method1',
        'len': 2
      },
      'method2': {
        'name': 'method2',
        'len': 1
      }
    },
    'Action2': {
      'method1': {
        'name': 'method1',
        'len': 1
      }
    }
  }
}");
		}

		private class ActionClass1
		{
			public void Method1(string p1, string p2) {}
			public void Method2(string p1) { }
		}

		private class ActionClass2
		{
			public void Method1(string p1) {}
		}
	}
}