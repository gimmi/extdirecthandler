using System.Collections.Generic;
using System.Reflection;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectApiBuilder
	{
		private readonly Metadata _metadata;

		public DirectApiBuilder(Metadata metadata)
		{
			_metadata = metadata;
		}

		internal JObject BuildApi(string ns, string url)
		{
			var api = new JObject{
				{ "id", new JValue(ns) },
				{ "url", new JValue(url) },
				{ "type", new JValue("remoting") },
				{ "namespace", new JValue(ns) },
				{ "actions", BuildActions() }
			};
			return api;
		}

		private JObject BuildActions()
		{
			var actions = new JObject();
			foreach(string actionName in _metadata.GetActionNames())
			{
				actions.Add(actionName, BuildMethods(actionName));
			}
			return actions;
		}

		private JObject BuildMethods(string actionName)
		{
			var methods = new JObject();
			foreach (string methodName in _metadata.GetMethodNames(actionName))
			{
				methods.Add(methodName, new JObject{
					{ "name", methodName },
					{ "len", _metadata.GetNumberOfParameters(actionName, methodName) }
				});
			}
			return methods;
		}
	}
}