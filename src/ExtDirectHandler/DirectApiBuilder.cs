using System;
using System.Linq;
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
			var api = new JObject {
				{ "id", new JValue(id) },
				{ "url", new JValue(url) },
				{ "type", new JValue("remoting") },
				{ "namespace", new JValue(ns) },
				{ "actions", BuildActions() }
			};
			return api;
		}

		internal JObject BuildApiDescriptor(string id, string ns, string url)
		{
			var api = new JObject {
				{ "id", new JValue(id) },
				{ "url", new JValue(url) },
				{ "type", new JValue("remoting") },
				{ "namespace", new JValue(ns) },
				{ "actions", BuildActions() },
				{ "descriptor", new JValue((ns ?? "ext.app") + ".REMOTING_API") }
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
				var obj = new JObject { { "name", methodName } };
				if(_metadata.IsFormHandler(actionName, methodName))
				{
					obj.Add("formHandler", new JValue(true));
				}
				if(_metadata.HasNamedArguments(actionName, methodName))
				{
					obj.Add("params", new JArray(_metadata.GetArgumentNames(actionName, methodName).Select(x => new JValue(x))));
				}
				else
				{
					obj.Add("len", new JValue(_metadata.GetNumberOfParameters(actionName, methodName)));
				}
				methods.Add(obj);
			}
			return methods;
		}
	}
}