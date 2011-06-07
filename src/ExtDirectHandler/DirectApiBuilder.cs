using System.Collections.Generic;
using System.Reflection;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectApiBuilder
	{
		private readonly IDictionary<string, DirectActionMetadata> _actionMetadatas;

		public DirectApiBuilder(IDictionary<string, DirectActionMetadata> actionMetadatas)
		{
			_actionMetadatas = actionMetadatas;
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
			foreach(KeyValuePair<string, DirectActionMetadata> kvp in _actionMetadatas)
			{
				actions.Add(kvp.Key, BuildMethods(kvp.Value.Methods));
			}
			return actions;
		}

		private JObject BuildMethods(IEnumerable<KeyValuePair<string, MethodInfo>> methodMetadatas)
		{
			var methods = new JObject();
			foreach (KeyValuePair<string, MethodInfo> methodMetadata in methodMetadatas)
			{
				methods.Add(methodMetadata.Key, new JObject{
					{ "name", methodMetadata.Key },
					{ "len", methodMetadata.Value.GetParameters().Length }
				});
			}
			return methods;
		}
	}
}