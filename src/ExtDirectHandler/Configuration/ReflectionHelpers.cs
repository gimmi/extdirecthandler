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
			return (T)member.GetCustomAttributes(typeof(T), false).FirstOrDefault();
		}

		public bool HasAttribute<T>(MemberInfo member) where T : Attribute
		{
			return FindAttribute<T>(member) != default(T);
		}

		public T GetAttribute<T>(MemberInfo member) where T : Attribute
		{
			var attribute = FindAttribute<T>(member);
			if(attribute == default(T))
			{
				throw new InvalidOperationException(string.Format("Member {0} must be decorated with {1}", member, typeof(T)));
			}
			return attribute;
		}
	}
}