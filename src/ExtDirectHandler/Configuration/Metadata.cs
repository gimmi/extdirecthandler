using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ExtDirectHandler.Configuration
{
	public class Metadata
	{
		private readonly IDictionary<string, ActionMetadata> _cache = new Dictionary<string, ActionMetadata>();

		public virtual void AddAction(string actionName, Type type)
		{
			_cache.Add(actionName, new ActionMetadata{ Type = type });
		}

		public virtual void AddMethod(string actionName, string methodName, MethodInfo methodInfo, bool isFormHandler, bool hasNamedArguments)
		{
			_cache[actionName].Methods.Add(methodName, new MethodMetadata{
				MethodInfo = methodInfo, 
				IsFormHandler = isFormHandler,
				HasNamedArguments = hasNamedArguments
			});
		}

		public virtual Type GetActionType(string actionName)
		{
			return _cache[actionName].Type;
		}

		public virtual IEnumerable<string> GetActionNames()
		{
			return _cache.Keys;
		}

		public virtual IEnumerable<string> GetMethodNames(string actionName)
		{
			return _cache[actionName].Methods.Keys;
		}

		public virtual MethodInfo GetMethodInfo(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].MethodInfo;
		}

		public virtual int GetNumberOfParameters(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].MethodInfo.GetParameters().Length;
		}

		public virtual bool IsFormHandler(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].IsFormHandler;
		}

		public virtual bool HasNamedArguments(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].HasNamedArguments;
		}

		public virtual IEnumerable<string> GetArgumentNames(string actionName, string methodName)
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