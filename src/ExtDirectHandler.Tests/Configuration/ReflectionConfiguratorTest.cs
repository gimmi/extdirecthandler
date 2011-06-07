using System.Collections.Generic;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace ExtDirectHandler.Tests.Configuration
{
	[TestFixture]
	internal class ReflectionConfiguratorTest
	{
		private ReflectionConfigurator _target;

		[SetUp]
		public void SetUp()
		{
			_target = new ReflectionConfigurator();
		}

		[Test]
		public void Should_configure_all_registered_types()
		{
			var metadata = MockRepository.GenerateMock<Metadata>();
			metadata.Expect(x => x.AddAction("ActionClass1", typeof(ActionClass1)));
			metadata.Expect(x => x.AddAction("ActionClass2", typeof(ActionClass2)));

			_target.RegisterType<ActionClass1>()
				.RegisterType<ActionClass2>()
				.FillMetadata(metadata);

			metadata.VerifyAllExpectations();
		}

		[Test]
		public void Should_configure_methods()
		{
			var metadata = MockRepository.GenerateMock<Metadata>();
			metadata.Expect(x => x.AddMethod("ActionClass1", "publicInstanceMethod", typeof(ActionClass1).GetMethod("PublicInstanceMethod")));
			metadata.Expect(x => x.AddMethod("ActionClass1", "methodWithParameters", typeof(ActionClass1).GetMethod("MethodWithParameters")));

			_target.RegisterType<ActionClass1>().FillMetadata(metadata);

			metadata.VerifyAllExpectations();
		}
		
		private class BaseClass
		{
			public void BaseMethod() {}
		}

		private class ActionClass1 : BaseClass
		{
			public static void StaticMethod() {}
			public void PublicInstanceMethod() {}
			private void PrivateInstanceMethod() {}
			protected void ProtectedInstanceMethod() {}
			internal void InternalInstanceMethod() {}
			public void MethodWithParameters(int intPar, string stringPar, bool boolPar) {}
		}

		private class ActionClass2 {}
	}
}