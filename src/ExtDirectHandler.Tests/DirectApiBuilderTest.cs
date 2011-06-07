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
		public void Should_build_api()
		{
			_metadata.Stub(x => x.GetActionNames()).Return(new[]{ "Action1", "Action2" });
			_metadata.Stub(x => x.GetMethodNames("Action1")).Return(new[]{ "method1", "method2" });
			_metadata.Stub(x => x.GetNumberOfParameters("Action1", "method1")).Return(2);
			_metadata.Stub(x => x.GetNumberOfParameters("Action1", "method2")).Return(1);
			_metadata.Stub(x => x.GetMethodNames("Action2")).Return(new[]{ "method1" });
			_metadata.Stub(x => x.GetNumberOfParameters("Action2", "method1")).Return(1);

			string actual = _target.BuildApi("namespace", "url").ToString().Replace('"', '\'');

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
	}
}