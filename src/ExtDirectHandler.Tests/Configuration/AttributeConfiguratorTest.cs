using System.Collections.Generic;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace ExtDirectHandler.Tests.Configuration
{
	[TestFixture]
	internal class AttributeConfiguratorTest
	{
		private AttributeConfigurator _target;

		[SetUp]
		public void SetUp()
		{
			_target = new AttributeConfigurator();
		}

		[Test]
		public void Should_build_api()
		{
			Dictionary<string, DirectActionMetadata> actual = _target.AddType<ActionClass1>().BuildMetadata();
			actual.Count.Should().Be.EqualTo(1);
			actual.ContainsKey("Action1").Should().Be.True();
			actual["Action1"].Methods.Keys.Should().Have.SameValuesAs(new[]{
				"baseMethod", 
				"methodWithParameters",
				"publicInstanceMethod", 
				"privateInstanceMethod", 
				"protectedInstanceMethod", 
				"internalInstanceMethod", 
				"methodWithParameters"
			});
		}


		private class BaseClass
		{
			[DirectMethod("baseMethod")]
			public void BaseMethod() { }
		}

		[DirectAction("Action1")]
		private class ActionClass1 :BaseClass
		{
			[DirectMethod("staticMethod")]
			public static void StaticMethod() {}

			[DirectMethod("publicInstanceMethod")]
			public void PublicInstanceMethod() {}

			[DirectMethod("privateInstanceMethod")]
			public void PrivateInstanceMethod() {}

			[DirectMethod("protectedInstanceMethod")]
			protected void ProtectedInstanceMethod() {}

			[DirectMethod("internalInstanceMethod")]
			internal void InternalInstanceMethod() {}

			[DirectMethod("methodWithParameters")]
			public void MethodWithParameters(int intPar, string stringPar, bool boolPar) {}

			public void NotDecoratedMethod() {}
		}
	}
}