using System;
using System.Text;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json;
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

		internal string BuildApi(string ns, string url)
		{
			return new StringBuilder()
				.AppendFormat("Ext.ns('{0}');", ns ?? "Ext.app").Append(Environment.NewLine)
				.AppendFormat("{0}.REMOTING_API = {1};", ns ?? "Ext.app", BuildApi(ns, ns, url).ToString(Formatting.Indented))
				.ToString();
		}

		internal JObject BuildApi(string id, string ns, string url)
		{
			var api = new JObject{
				{ "id", new JValue(id) },
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

		private JArray BuildMethods(string actionName)
		{
			var methods = new JArray();
			foreach(string methodName in _metadata.GetMethodNames(actionName))
			{
				methods.Add(new JObject{
					{ "name", methodName },
					{ "len", _metadata.GetNumberOfParameters(actionName, methodName) }
				});
			}
			return methods;
		}
	}
}