using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class ReflectionConfigurator : IMetadata
	{
		private readonly IDictionary<string, IDictionary<string, MethodMetadata>> _cache = new Dictionary<string, IDictionary<string, MethodMetadata>>();
		private readonly ReflectionHelpers _reflectionHelpers;
		private string _namespace;
		private string _id;
		private bool _preserveMethodCase;

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
			AddAction(type.Name);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(mi => mi.DeclaringType == type || _reflectionHelpers.HasAttribute<DirectMethodAttribute>(mi));
			foreach(MethodInfo methodInfo in methods)
			{
				DirectMethodAttribute directMethodAttribute = _reflectionHelpers.FindAttribute(methodInfo, new DirectMethodAttribute());
				AddMethod(type.Name, BuildMethodName(methodInfo.Name), type, methodInfo, directMethodAttribute.FormHandler, directMethodAttribute.NamedArguments);
			}
			return this;
		}

		public ReflectionConfigurator PreserveMethodCase(bool preserveMethodCase)
		{
			_preserveMethodCase = preserveMethodCase;
			return this;
		}

		public ReflectionConfigurator SetNamespace(string ns)
		{
			_namespace = ns;
			return this;
		}

		public ReflectionConfigurator SetId(string id)
		{
			_id = id;
			return this;
		}

		private void AddAction(string actionName)
		{
			_cache.Add(actionName, new Dictionary<string, MethodMetadata>());
		}

		private void AddMethod(string actionName, string methodName, Type type, MethodInfo methodInfo, bool isFormHandler, bool hasNamedArguments)
		{
			_cache[actionName].Add(methodName, new MethodMetadata {
				Type = type,
				MethodInfo = methodInfo,
				IsFormHandler = isFormHandler,
				HasNamedArguments = hasNamedArguments
			});
		}

		public string GetNamespace()
		{
			return _namespace;
		}

		public string GetId()
		{
			return _id;
		}

		public IEnumerable<string> GetActionNames()
		{
			return _cache.Keys;
		}

		public IEnumerable<string> GetMethodNames(string actionName)
		{
			return _cache[actionName].Keys;
		}

		public Type GetActionType(string actionName, string methodName)
		{
			return _cache[actionName][methodName].Type;
		}

		public MethodInfo GetMethodInfo(string actionName, string methodName)
		{
			return _cache[actionName][methodName].MethodInfo;
		}

		public int GetNumberOfParameters(string actionName, string methodName)
		{
			return _cache[actionName][methodName].MethodInfo.GetParameters().Length;
		}

		public bool IsFormHandler(string actionName, string methodName)
		{
			return _cache[actionName][methodName].IsFormHandler;
		}

		public bool HasNamedArguments(string actionName, string methodName)
		{
			return _cache[actionName][methodName].HasNamedArguments;
		}

		public IEnumerable<string> GetArgumentNames(string actionName, string methodName)
		{
			return _cache[actionName][methodName].MethodInfo.GetParameters().Select(p => p.Name);
		}

		private string BuildMethodName(string name)
		{
			if(_preserveMethodCase)
			{
				return name;
			}
			return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
		}

		#region Nested type: MethodMetadata

		private class MethodMetadata
		{
			public bool HasNamedArguments;
			public bool IsFormHandler;
			public MethodInfo MethodInfo;
			public Type Type;
		}

		#endregion
	}
}