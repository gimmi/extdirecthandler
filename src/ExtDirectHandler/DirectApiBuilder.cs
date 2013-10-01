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
		private readonly IMetadata _metadata;

		public DirectApiBuilder(IMetadata metadata)
		{
			_metadata = metadata;
		}

		internal string BuildJavascript(string url)
		{
			return new StringBuilder()
				.AppendFormat("Ext.ns('{0}');", GetApiDescriptorNamespace(_metadata.Namespace)).Append(Environment.NewLine)
				.AppendFormat("{0} = {1};", GetApiDescriptorName(_metadata.Namespace, _metadata.Id), BuildJson(url))
				.ToString();
		}

		internal string BuildJson(string url)
		{
			return new JObject {
				{ "id", new JValue(GetApiDescriptorId(_metadata.Id)) },
				{ "url", new JValue(url) },
				{ "type", new JValue("remoting") },
				{ "namespace", new JValue(_metadata.Namespace) },
				{ "actions", BuildActions() },
				// "descriptor" is needed for integrating with Ext Designer
				// see http://davehiren.blogspot.com/2011/03/configure-extdirect-api-with-ext.html
				// see http://www.sencha.com/forum/showthread.php?102357#post_message_480214
				{ "descriptor", new JValue(GetApiDescriptorName(_metadata.Namespace, _metadata.Id)) }
			}.ToString(Formatting.Indented);
		}

		private string GetApiDescriptorName(string ns, string id)
		{
			return string.Format("{0}.{1}", GetApiDescriptorNamespace(ns), GetApiDescriptorId(id));
		}

		private string GetApiDescriptorNamespace(string ns)
		{
			return ns ?? "Ext.app";
		}

		private string GetApiDescriptorId(string id)
		{
			return id ?? "REMOTING_API";
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