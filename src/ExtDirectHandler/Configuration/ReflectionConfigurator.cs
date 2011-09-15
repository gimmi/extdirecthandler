using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class ReflectionConfigurator : IMetadata
	{
		private readonly IDictionary<string, ActionMetadata> _cache = new Dictionary<string, ActionMetadata>();
		private string _namespace;
		private readonly ReflectionHelpers _reflectionHelpers;

		internal ReflectionConfigurator(ReflectionHelpers reflectionHelpers)
		{
			_reflectionHelpers = reflectionHelpers;
		}

		public ReflectionConfigurator() : this(new ReflectionHelpers()) {}

		public ReflectionConfigurator RegisterTypes(params Type[] types)
		{
			return RegisterTypes(types.AsEnumerable());
		}

		public ReflectionConfigurator RegisterType<T>()
		{
			return RegisterType(typeof(T));
		}

		public ReflectionConfigurator RegisterTypes(IEnumerable<Type> types)
		{
			foreach(Type type in types)
			{
				RegisterType(type);
			}
			return this;
		}

		public ReflectionConfigurator RegisterType(Type type)
		{
			string actionName = type.Name;
			AddAction(actionName, type);
			foreach(MethodInfo methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
			{
				DirectMethodAttribute directMethodAttribute = _reflectionHelpers.FindAttribute(methodInfo, new DirectMethodAttribute());
				AddMethod(actionName, pascalizeName(methodInfo.Name), methodInfo, directMethodAttribute.FormHandler, directMethodAttribute.NamedArguments);
			}
			return this;
		}

		private string pascalizeName(string name)
		{
			return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
		}

		public ReflectionConfigurator SetNamespace(string ns)
		{
			_namespace = ns;
			return this;
		}

		private void AddAction(string actionName, Type type)
		{
			_cache.Add(actionName, new ActionMetadata { Type = type });
		}

		private void AddMethod(string actionName, string methodName, MethodInfo methodInfo, bool isFormHandler, bool hasNamedArguments)
		{
			_cache[actionName].Methods.Add(methodName, new MethodMetadata {
				MethodInfo = methodInfo,
				IsFormHandler = isFormHandler,
				HasNamedArguments = hasNamedArguments
			});
		}

		public string GetNamespace()
		{
			return _namespace;
		}

		public Type GetActionType(string actionName)
		{
			return _cache[actionName].Type;
		}

		public IEnumerable<string> GetActionNames()
		{
			return _cache.Keys;
		}

		public IEnumerable<string> GetMethodNames(string actionName)
		{
			return _cache[actionName].Methods.Keys;
		}

		public MethodInfo GetMethodInfo(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].MethodInfo;
		}

		public int GetNumberOfParameters(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].MethodInfo.GetParameters().Length;
		}

		public bool IsFormHandler(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].IsFormHandler;
		}

		public bool HasNamedArguments(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].HasNamedArguments;
		}

		public IEnumerable<string> GetArgumentNames(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].MethodInfo.GetParameters().Select(p => p.Name);
		}

		#region Nested type: ActionMetadata

		private class ActionMetadata
		{
			public readonly IDictionary<string, MethodMetadata> Methods = new Dictionary<string, MethodMetadata>();
			public Type Type;
		}

		#endregion

		#region Nested type: MethodMetadata

		private class MethodMetadata
		{
			public MethodInfo MethodInfo;
			public bool IsFormHandler;
			public bool HasNamedArguments;
		}

		#endregion
	}
}