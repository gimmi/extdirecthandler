using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtDirectHandler
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
		public JToken Result;

		private DirectResponse(DirectRequest request)
		{
			Action = request.Action;
			Method = request.Method;
			Tid = request.Tid;
			Type = request.Type;
		}

		public DirectResponse(DirectRequest request, Exception e) : this(request)
		{
			Type = "exception";
			Message = e.Message;
			Where = e.ToString();
		}

		public DirectResponse(DirectRequest request, JToken result) : this(request)
		{
			Result = result;
		}
	}
}