using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpikeHttpHandler
{
	internal class DirectRequest
	{
		[JsonProperty("method")]
		public String Method;

		[JsonProperty("type")]
		public String Type;

		[JsonProperty("tid")]
		public int Tid;

		[JsonProperty("data")]
		public JToken[] Data;

		[JsonProperty("action")]
		public String Action;
	}
}