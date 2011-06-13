using System;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace ExtDirectHandler.Tests.Configuration
{
	[TestFixture]
	public class ReflectionHelpersTest
	{
		private ReflectionHelpers _target;

		[SetUp]
		public void SetUp()
		{
			_target = new ReflectionHelpers();
		}

		[Test]
		public void Should_find_attribute_when_present()
		{
			_target.HasAttribute<MyAttribute>(typeof(MyClass)).Should().Be.True();
			_target.FindAttribute<MyAttribute>(typeof(MyClass)).Should().Be.OfType<MyAttribute>();
		}

		[Test]
		public void Should_not_find_attribute_when_absent()
		{
			_target.HasAttribute<MyAttribute>(typeof(MyUndecoratedClass)).Should().Be.False();
			_target.FindAttribute<MyAttribute>(typeof(MyUndecoratedClass)).Should().Be.Null();
		}

		public class MyAttribute : Attribute {}

		[My]
		private class MyClass {}

		private class MyUndecoratedClass {}
	}
}