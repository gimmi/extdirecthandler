using System.Collections.Generic;
using ExtDirectHandler.Configuration;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectApiBuilder
	{
		private readonly IEnumerable<DirectActionMetadata> _actionMetadatas;

		public DirectApiBuilder(IEnumerable<DirectActionMetadata> actionMetadatas)
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
			foreach(DirectActionMetadata actionMetadata in _actionMetadatas)
			{
				actions.Add(actionMetadata.Name, BuildMethods(actionMetadata.Methods.Values));
			}
			return actions;
		}

		private JObject BuildMethods(IEnumerable<DirectMethodMetadata> methodMetadatas)
		{
			var methods = new JObject();
			foreach(DirectMethodMetadata methodMetadata in methodMetadatas)
			{
				methods.Add(methodMetadata.Name, new JObject{
					{ "name", methodMetadata.Name },
					{ "len", methodMetadata.Method.GetParameters().Length }
				});
			}
			return methods;
		}
	}
}