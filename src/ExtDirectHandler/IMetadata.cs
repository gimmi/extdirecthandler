using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtDirectHandler
{
	public interface IMetadata
	{
		string GetNamespace();
		Type GetActionType(string actionName);
		IEnumerable<string> GetActionNames();
		IEnumerable<string> GetMethodNames(string actionName);
		MethodInfo GetMethodInfo(string actionName, string methodName);
		int GetNumberOfParameters(string actionName, string methodName);
		bool IsFormHandler(string actionName, string methodName);
		bool HasNamedArguments(string actionName, string methodName);
		IEnumerable<string> GetArgumentNames(string actionName, string methodName);
	}
}