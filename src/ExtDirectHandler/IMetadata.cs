using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtDirectHandler
{
	public interface IMetadata
	{
		string Namespace { get; }
		string Id { get; }
		bool? EnableBuffer { get; }
		int? BufferTimeout { get; }
		IEnumerable<string> GetActionNames();
		IEnumerable<string> GetMethodNames(string actionName);
		Type GetActionType(string actionName, string methodName);
		MethodInfo GetMethodInfo(string actionName, string methodName);
		int GetNumberOfParameters(string actionName, string methodName);
		bool IsFormHandler(string actionName, string methodName);
		bool HasNamedArguments(string actionName, string methodName);
		IEnumerable<string> GetArgumentNames(string actionName, string methodName);
	}
}