using System;
using ExtDirectHandler.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace ExtDirectHandler.Tests.Configuration
{
	[TestFixture]
	public class LambdaConfiguratorTest
	{
		private LambdaConfigurator _target;

		[SetUp]
		public void SetUp()
		{
			_target = new LambdaConfigurator();
		}

		[Test]
		public void Should_register_methods_with_no_parameters_and_no_return_value()
		{
			_target.Register<TestClass>("Service.noParNoRet", x => x.NoParNoRet());
			_target.GetMethodInfo("Service", "noParNoRet").Should().Be.SameInstanceAs(typeof(TestClass).GetMethod("NoParNoRet"));
		}

		[Test]
		public void Should_register_methods_with_no_parameters_and_return_value()
		{
			_target.Register<TestClass>("Service.noParWithRet", x => x.NoParWithRet());
			_target.GetMethodInfo("Service", "noParWithRet").Should().Be.SameInstanceAs(typeof(TestClass).GetMethod("NoParWithRet"));
		}

		[Test]
		public void Should_register_methods_with_parameters_and_no_return_value()
		{
			_target.Register<TestClass>("Service.withParNoRet", x => x.WithParNoRet(default(int), default(string)));
			_target.GetMethodInfo("Service", "withParNoRet").Should().Be.SameInstanceAs(typeof(TestClass).GetMethod("WithParNoRet"));
		}

		[Test]
		public void Should_register_methods_with_parameters_and_return_value()
		{
			_target.Register<TestClass>("Service.withParWithRet", x => x.WithParWithRet(default(int), default(string)));
			_target.GetMethodInfo("Service", "withParWithRet").Should().Be.SameInstanceAs(typeof(TestClass).GetMethod("WithParWithRet"));
		}

		[Test]
		public void Should_throw_exception_when_method_not_registered()
		{
			Executing.This(() => _target.GetMethodInfo("Action", "NotRegistered")).Should().Throw<ArgumentException>().And.ValueOf.Message.Should().Be.EqualTo("Method 'Action.NotRegistered' not registered");
		}

		[Test]
		public void Should_throw_exception_when_name_not_in_the_expected_format()
		{
			Executing.This(() => _target.Register<TestClass>("method", x => x.NoParNoRet())).Should().Throw<ArgumentException>().And.ValueOf.Message.Should().Be.EqualTo("Invalid name 'method'. Name must be in the form 'Action.method'");
			Executing.This(() => _target.Register<TestClass>("Ns.Action.method", x => x.NoParNoRet())).Should().Throw<ArgumentException>().And.ValueOf.Message.Should().Be.EqualTo("Invalid name 'Ns.Action.method'. Name must be in the form 'Action.method'");
		}

		[Test]
		public void Should_throw_exception_when_registering_the_same_method_name_multiple_times()
		{
			_target.Register<TestClass>("Action.method", x => x.NoParNoRet());
			Executing.This(() => _target.Register<TestClass>("Action.method", x => x.NoParNoRet())).Should().Throw<ArgumentException>().And.ValueOf.Message.Should().Be.EqualTo("Method 'Action.method' already registered");
		}

		[Test]
		public void Should_register_passed_type()
		{
			_target.Register<TestClass>("Action.methodFromBase", x => x.ToString());
			_target.Register<TestClass>("Action.overriddenMethod", x => x.GetHashCode());
			_target.GetActionType("Action", "methodFromBase").Should().Be.EqualTo(typeof(TestClass));
			_target.GetActionType("Action", "overriddenMethod").Should().Be.EqualTo(typeof(TestClass));
		}

		public class TestClass
		{
			public void NoParNoRet() {}

			public int NoParWithRet()
			{
				return 0;
			}

			public void WithParNoRet(int p1, string p2) {}

			public string WithParWithRet(int p1, string p2)
			{
				return null;
			}

			public override int GetHashCode()
			{
				return 1;
			}
		}
	}
}