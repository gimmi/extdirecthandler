using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpikeHttpHandler.Configuration
{
	public class DirectActionMetadata
	{
		public Type Type;
		public string Name;
		public IDictionary<string, DirectMethodMetadata> Methods = new Dictionary<string, DirectMethodMetadata>();
	}
}