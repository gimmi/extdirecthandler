using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class LambdaConfigurator : IMetadata
	{
		private readonly IDictionary<string, IDictionary<string, MethodMetadata>> _cache = new Dictionary<string, IDictionary<string, MethodMetadata>>();
		private string _namespace;

		#region IMetadata Members

		public string GetNamespace()
		{
			return _namespace;
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
			return GetMethodMetadata(actionName, methodName).Type;
		}

		public MethodInfo GetMethodInfo(string actionName, string methodName)
		{
			return GetMethodMetadata(actionName, methodName).MethodInfo;
		}

		public int GetNumberOfParameters(string actionName, string methodName)
		{
			return GetMethodMetadata(actionName, methodName).MethodInfo.GetParameters().Length;
		}

		public bool IsFormHandler(string actionName, string methodName)
		{
			return GetMethodMetadata(actionName, methodName).IsFormHandler;
		}

		public bool HasNamedArguments(string actionName, string methodName)
		{
			return GetMethodMetadata(actionName, methodName).HasNamedArguments;
		}

		public IEnumerable<string> GetArgumentNames(string actionName, string methodName)
		{
			return GetMethodMetadata(actionName, methodName).MethodInfo.GetParameters().Select(p => p.Name);
		}

		#endregion

		public LambdaConfigurator Register<T>(string name, Expression<Action<T>> expression, bool namedArguments = false, bool formHandler = false)
		{
			return Register(name, typeof(T), expression, namedArguments, formHandler);
		}

		public LambdaConfigurator Register<T, TResult>(string name, Expression<Func<T, TResult>> expression, bool namedArguments = false, bool formHandler = false)
		{
			return Register(name, typeof(T), expression, namedArguments, formHandler);
		}

		public LambdaConfigurator Register(string name, Type type, LambdaExpression expression, bool namedArguments = false, bool formHandler = false)
		{
			var outermostExpression = expression.Body as MethodCallExpression;
			if(outermostExpression == null)
			{
				throw new ArgumentException("Invalid Expression. Expression must consist of a Method call only.");
			}
			string[] nameParts = name.Split('.');
			if(nameParts.Length != 2)
			{
				throw new ArgumentException(string.Format("Invalid name '{0}'. Name must be in the form 'Action.method'", name));
			}
			if(!_cache.ContainsKey(nameParts[0]))
			{
				_cache.Add(nameParts[0], new Dictionary<string, MethodMetadata>());
			}
			if (_cache[nameParts[0]].ContainsKey(nameParts[1]))
			{
				throw new ArgumentException(string.Format("Method '{0}.{1}' already registered", nameParts[0], nameParts[1]));
			}
			_cache[nameParts[0]].Add(nameParts[1], new MethodMetadata {
				Type = type,
				MethodInfo = outermostExpression.Method,
				HasNamedArguments = namedArguments,
				IsFormHandler = formHandler
			});
			return this;
		}

		public LambdaConfigurator SetNamespace(string ns)
		{
			_namespace = ns;
			return this;
		}

		private MethodMetadata GetMethodMetadata(string actionName, string methodName)
		{
			try
			{
				return _cache[actionName][methodName];
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentException(string.Format("Method '{0}.{1}' not registered", actionName, methodName));
			}
		}

		#region Nested type: MethodMetadata

		private class MethodMetadata
		{
			public Type Type;
			public bool HasNamedArguments;
			public bool IsFormHandler;
			public MethodInfo MethodInfo;
		}

		#endregion
	}
}