using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class Metadata
	{
		private class ActionMetadata
		{
			public Type Type;
			public readonly IDictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
		}

		private static readonly IDictionary<string, ActionMetadata> _cache = new Dictionary<string, ActionMetadata>();

		public virtual void AddAction(string actionName, Type type)
		{
			_cache.Add(actionName, new ActionMetadata{ Type = type });
		}

		public virtual void AddMethod(string actionName, string methodName, MethodInfo methodInfo)
		{
			_cache[actionName].Methods.Add(methodName, methodInfo);
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
			return _cache[actionName].Methods[methodName];
		}

		public virtual int GetNumberOfParameters(string actionName, string methodName)
		{
			return _cache[actionName].Methods[methodName].GetParameters().Length;
		}

		public virtual bool IsFormHandler(string actionName, string methodName)
		{
			return (actionName == "Profile" && methodName == "updateBasicInfo"); // TODO this is just to try Ext examples
		}
	}
}