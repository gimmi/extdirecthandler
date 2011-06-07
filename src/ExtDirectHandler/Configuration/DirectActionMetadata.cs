using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	internal class DirectActionMetadata
	{
		public Type Type;
		public IDictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
	}
}