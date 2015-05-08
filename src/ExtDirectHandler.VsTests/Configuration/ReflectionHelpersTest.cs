using System;
using ExtDirectHandler.Configuration;
using SharpTestsEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtDirectHandler.VsTests.Configuration
{
    [TestClass]
    public class ReflectionHelpersTest
    {
        private ReflectionHelpers _target;

        [TestInitialize]
        public void SetUp()
        {
            _target = new ReflectionHelpers();
        }

        [TestMethod]
        public void Should_find_attribute_when_present()
        {
            _target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("DecoratedMethod")).Should().Be.True();
            _target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("DecoratedMethod")).Should().Be.OfType<SampleAttribute>();
        }

        [TestMethod]
        public void Should_not_find_attribute_when_absent()
        {
            _target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("UndecoratedMethod")).Should().Be.False();
            _target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("UndecoratedMethod")).Should().Be.Null();
        }

        [TestMethod]
        public void Should_find_inherited_attributes()
        {
            _target.HasAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("OverriddenMethod")).Should().Be.True();
            _target.FindAttribute<SampleAttribute>(typeof(SampleClass).GetMethod("OverriddenMethod")).Should().Be.OfType<SampleAttribute>();
        }

        public class SampleAttribute : Attribute { }

        private class SampleBaseClass
        {
            [Sample]
            public virtual void OverriddenMethod() { }
        }

        private class SampleClass : SampleBaseClass
        {
            [Sample]
            public void DecoratedMethod() { }

            public void UndecoratedMethod() { }

            public override void OverriddenMethod() { }
        }
    }
}