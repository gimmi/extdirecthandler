using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class Metadata
	{
		private static readonly IDictionary<string, ActionMetadata> Cache = new Dictionary<string, ActionMetadata>();

		public virtual void AddAction(string actionName, Type type)
		{
			Cache.Add(actionName, new ActionMetadata{ Type = type });
		}

		public virtual void AddMethod(string actionName, string methodName, MethodInfo methodInfo)
		{
			Cache[actionName].Methods.Add(methodName, new MethodMetadata{ MethodInfo = methodInfo });
		}

		public virtual Type GetActionType(string actionName)
		{
			return Cache[actionName].Type;
		}

		public virtual IEnumerable<string> GetActionNames()
		{
			return Cache.Keys;
		}

		public virtual IEnumerable<string> GetMethodNames(string actionName)
		{
			return Cache[actionName].Methods.Keys;
		}

		public virtual MethodInfo GetMethodInfo(string actionName, string methodName)
		{
			return Cache[actionName].Methods[methodName].MethodInfo;
		}

		public virtual int GetNumberOfParameters(string actionName, string methodName)
		{
			return Cache[actionName].Methods[methodName].MethodInfo.GetParameters().Length;
		}

		public virtual bool IsFormHandler(string actionName, string methodName)
		{
			return (actionName == "Profile" && methodName == "updateBasicInfo"); // TODO this is just to try Ext examples
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
		}

		#endregion
	}
}