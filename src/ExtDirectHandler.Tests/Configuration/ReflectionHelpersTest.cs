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
			_target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("DecoratedMethod")).Should().Be.True();
			_target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("DecoratedMethod")).Should().Be.OfType<SampleAttribute>();
		}

		[Test]
		public void Should_not_find_attribute_when_absent()
		{
			_target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("UndecoratedMethod")).Should().Be.False();
			_target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("UndecoratedMethod")).Should().Be.Null();
		}

		[Test]
		public void Should_find_inherited_attributes()
		{
			_target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("OverriddenMethod")).Should().Be.True();
			_target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("OverriddenMethod")).Should().Be.OfType<SampleAttribute>();
		}

		public class SampleAttribute : Attribute {}

		private class SampleBaseClass
		{
			[Sample]
			public virtual void OverriddenMethod() {}
		}

		private class SampleClass : SampleBaseClass
		{
			[Sample]
			public void DecoratedMethod() {}

			public void UndecoratedMethod() {}

			public override void OverriddenMethod() {}
		}
	}
}