using ExtDirectHandler.Configuration;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace ExtDirectHandler.Tests
{
	[TestFixture]
	public class DirectApiBuilderTest
	{
		private Metadata _metadata;
		private DirectApiBuilder _target;

		[SetUp]
		public void SetUp()
		{
			_metadata = MockRepository.GenerateStub<Metadata>();
			_target = new DirectApiBuilder(_metadata);
		}

		[Test]
		public void Should_use_default_place_for_api_and_no_namespace_when_no_namespace_provided()
		{
			_metadata.Stub(x => x.GetActionNames()).Return(new string[0]);
			string actual = _target.BuildApi(null, "http://localhost:8080/rpc").Replace('"', '\'');

			actual.Should().Be.EqualTo(@"Ext.ns('Ext.app');
Ext.app.REMOTING_API = {
  'id': null,
  'url': 'http://localhost:8080/rpc',
  'type': 'remoting',
  'namespace': null,
  'actions': {}
};");
			
		}

		[Test]
		public void Should_build_api()
		{
			_metadata.Stub(x => x.GetActionNames()).Return(new[]{ "Action1", "Action2" });
			_metadata.Stub(x => x.GetMethodNames("Action1")).Return(new[]{ "method1", "method2" });
			_metadata.Stub(x => x.GetNumberOfParameters("Action1", "method1")).Return(2);
			_metadata.Stub(x => x.IsFormHandler("Action1", "method1")).Return(false);
			_metadata.Stub(x => x.GetNumberOfParameters("Action1", "method2")).Return(1);
			_metadata.Stub(x => x.IsFormHandler("Action1", "method2")).Return(true);
			_metadata.Stub(x => x.GetMethodNames("Action2")).Return(new[]{ "method1" });
			_metadata.Stub(x => x.GetNumberOfParameters("Action2", "method1")).Return(1);
			_metadata.Stub(x => x.IsFormHandler("Action2", "method1")).Return(false);

			string actual = _target.BuildApi("App.server", "http://localhost:8080/rpc").Replace('"', '\'');

			actual.Should().Be.EqualTo(@"Ext.ns('App.server');
App.server.REMOTING_API = {
  'id': 'App.server',
  'url': 'http://localhost:8080/rpc',
  'type': 'remoting',
  'namespace': 'App.server',
  'actions': {
    'Action1': [
      {
        'name': 'method1',
        'formHandler': false,
        'len': 2
      },
      {
        'name': 'method2',
        'formHandler': true,
        'len': 1
      }
    ],
    'Action2': [
      {
        'name': 'method1',
        'formHandler': false,
        'len': 1
      }
    ]
  }
};");
		}
	}
}