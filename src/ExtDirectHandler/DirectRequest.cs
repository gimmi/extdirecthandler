using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectRequest
	{
		[JsonProperty("method")]
		public String Method;

		[JsonProperty("type")]
		public String Type;

		[JsonProperty("tid")]
		public int Tid;

		[JsonProperty("action")]
		public String Action;

		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public JToken JsonData = new JArray();

		public IDictionary<string, string> FormData = new Dictionary<string, string>();

		public bool Upload;
	}
}