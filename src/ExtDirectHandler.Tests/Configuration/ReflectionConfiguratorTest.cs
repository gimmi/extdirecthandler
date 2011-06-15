using ExtDirectHandler.Configuration;
using NUnit.Framework;
using Rhino.Mocks;

namespace ExtDirectHandler.Tests.Configuration
{
	[TestFixture]
	internal class ReflectionConfiguratorTest
	{
		private ReflectionHelpers _reflectionHelpers;
		private ReflectionConfigurator _target;

		[SetUp]
		public void SetUp()
		{
			_reflectionHelpers = new ReflectionHelpers();
			_target = new ReflectionConfigurator(_reflectionHelpers);
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
			metadata.Expect(x => x.AddMethod("ActionClass1", "publicInstanceMethod", typeof(ActionClass1).GetMethod("PublicInstanceMethod"), false, false));
			metadata.Expect(x => x.AddMethod("ActionClass1", "methodWithParameters", typeof(ActionClass1).GetMethod("MethodWithParameters"), false, false));

			_target.RegisterType<ActionClass1>().FillMetadata(metadata);

			metadata.VerifyAllExpectations();
		}

		[Test]
		public void Should_configure_formhandler_from_attribute()
		{
			var metadata = MockRepository.GenerateMock<Metadata>();
			metadata.Expect(x => x.AddMethod("ActionClass3", "formHandlerMethod", typeof(ActionClass3).GetMethod("FormHandlerMethod"), true, false));

			_target.RegisterType<ActionClass3>().FillMetadata(metadata);

			metadata.VerifyAllExpectations();
		}

		[Test]
		public void Should_configure_named_arguments_from_attribute()
		{
			var metadata = MockRepository.GenerateMock<Metadata>();
			metadata.Expect(x => x.AddMethod("ActionClass3", "namedArgumentsMethod", typeof(ActionClass3).GetMethod("NamedArgumentsMethod"), false, true));

			_target.RegisterType<ActionClass3>().FillMetadata(metadata);

			metadata.VerifyAllExpectations();
		}

		private class ActionClass3
		{
			[DirectMethod(FormHandler = true)]
			public void FormHandlerMethod() {}

			[DirectMethod(NamedArguments = true)]
			public void NamedArgumentsMethod() {}
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