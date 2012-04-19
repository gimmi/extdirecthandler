using ExtDirectHandler.Configuration;
using NUnit.Framework;
using SharpTestsEx;

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
		public void Should_return_null_if_no_namespace_defined()
		{
			_target.GetNamespace().Should().Be.Null();
		}

		[Test]
		public void Should_configure_namespace()
		{
			_target.SetNamespace("ns");

			_target.GetNamespace().Should().Be.EqualTo("ns");
		}

		[Test]
		public void Should_configure_all_registered_types()
		{
			_target.RegisterType<ActionClass1>()
				.RegisterType<ActionClass2>();

			_target.GetActionNames().Should().Have.SameValuesAs(new[] { "ActionClass1", "ActionClass2" });
		}

		[Test]
		public void Should_configure_methods()
		{
			IMetadata actual = _target.RegisterType<ActionClass1>();

			_target.GetMethodNames("ActionClass1").Should().Have.SameValuesAs(new[] { "publicInstanceMethod", "methodWithParameters" });

			_target.GetMethodInfo("ActionClass1", "publicInstanceMethod").Should().Be.SameInstanceAs(typeof(ActionClass1).GetMethod("PublicInstanceMethod"));
			_target.IsFormHandler("ActionClass1", "publicInstanceMethod").Should().Be.False();
			_target.HasNamedArguments("ActionClass1", "publicInstanceMethod").Should().Be.False();

			_target.GetMethodInfo("ActionClass1", "methodWithParameters").Should().Be.SameInstanceAs(typeof(ActionClass1).GetMethod("MethodWithParameters"));
			_target.IsFormHandler("ActionClass1", "methodWithParameters").Should().Be.False();
			_target.HasNamedArguments("ActionClass1", "methodWithParameters").Should().Be.False();
		}

		[Test]
		public void Should_configure_methods_with_inheritance()
		{
			IMetadata actual = _target.RegisterType<ActionClass1>(true);
			_target.GetMethodNames("ActionClass1").Should().Contain("baseMethod");
			_target.GetMethodNames("ActionClass1").Should().Not.Contain("getHashCode");
			_target.GetMethodNames("ActionClass1").Should().Not.Contain("getType");
		}

		[Test]
		public void Should_configure_formhandler_from_attribute()
		{
			IMetadata actual = _target.RegisterType<ActionClass3>();

			_target.IsFormHandler("ActionClass3", "formHandlerMethod").Should().Be.True();
		}

		[Test]
		public void Should_configure_named_arguments_from_attribute()
		{
			IMetadata actual = _target.RegisterType<ActionClass3>();

			_target.HasNamedArguments("ActionClass3", "namedArgumentsMethod").Should().Be.True();
		}

		[Test]
		public void Should_preserve_method_case()
		{
			_target.PreserveMethodCase(true).RegisterType<ActionClass1>();

			_target.GetMethodNames("ActionClass1").Should().Have.SameValuesAs(new[] { "PublicInstanceMethod", "MethodWithParameters" });
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