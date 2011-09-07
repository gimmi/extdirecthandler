using System;
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

		public bool Upload;
	}
}