using System;
using System.Collections.Generic;

namespace ExtDirectHandler.Configuration
{
	internal class DirectActionMetadata
	{
		public Type Type;
		public string Name;
		public IDictionary<string, DirectMethodMetadata> Methods = new Dictionary<string, DirectMethodMetadata>();
	}
}