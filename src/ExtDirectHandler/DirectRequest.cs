using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
{
	internal class DirectRequest
	{
		private JToken[] _data;

		[JsonProperty("method")]
		public String Method;

		[JsonProperty("type")]
		public String Type;

		[JsonProperty("tid")]
		public int Tid;

		[JsonProperty("action")]
		public String Action;

		[JsonIgnore]
		public bool Upload;

		[JsonProperty("data")]
		public JToken[] Data
		{
			get { return _data; }
			set { _data = value ?? new JToken[0]; }
		}
	}
}