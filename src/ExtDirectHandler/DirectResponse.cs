using System;
using Newtonsoft.Json;

namespace SpikeHttpHandler
{
	internal class DirectResponse
	{
		[JsonProperty("action")]
		public String Action;

		[JsonProperty("method")]
		public String Method;

		[JsonProperty("tid")]
		public int Tid;

		[JsonProperty("type")]
		public String Type;

		[JsonProperty("message")]
		public String Message;

		[JsonProperty("where")]
		public String Where;

		[JsonProperty("result")]
		public object Result;

		public DirectResponse(DirectRequest request)
		{
			Action = request.Action;
			Method = request.Method;
			Tid = request.Tid;
		}
	}
}