using System.Collections.Generic;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
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
			Dictionary<string, DirectActionMetadata> actual = _target.RegisterType<ActionClass1>().RegisterType<ActionClass2>().BuildMetadata();
			actual.Keys.Should().Have.SameValuesAs(new[]{ "ActionClass1", "ActionClass2" });
		}

		[Test]
		public void Should_configure_methods()
		{
			Dictionary<string, DirectActionMetadata> actual = _target.RegisterType<ActionClass1>().BuildMetadata();
			actual["ActionClass1"].Methods.Keys.Should().Have.SameValuesAs(new[]{
				"publicInstanceMethod",
				"methodWithParameters"
			});
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