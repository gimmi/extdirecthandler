using System;
using System.Linq;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	internal class ReflectionHelpers
	{
		public T FindAttribute<T>(MemberInfo member, T def) where T : Attribute
		{
			return FindAttribute<T>(member) ?? def;
		}

		public T FindAttribute<T>(MemberInfo member) where T : Attribute
		{
			return (T)member.GetCustomAttributes(typeof(T), true).FirstOrDefault();
		}

		public bool HasAttribute<T>(MemberInfo member) where T : Attribute
		{
			return FindAttribute<T>(member) != default(T);
		}
	}
}
