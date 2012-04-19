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

		public T FindAttribute<T>(MemberInfo member, T def, bool inherit) where T : Attribute
		{
			return FindAttribute<T>(member, inherit) ?? def;
		}

		public T FindAttribute<T>(MemberInfo member) where T : Attribute
		{
			return (T)member.GetCustomAttributes(typeof(T), false).FirstOrDefault();
		}

		public T FindAttribute<T>(MemberInfo member, bool inherit) where T : Attribute
		{
			return (T)member.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
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

		public MethodInfo[] GetInheritedMethods(Type type)
		{
			MethodInfo[] objectMethods = typeof(Object).GetMethods();
			MethodInfo[] result = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.TakeWhile(tm => !objectMethods.Contains(tm.GetBaseDefinition()))
				.ToArray();
			return result;
		}
	}
}