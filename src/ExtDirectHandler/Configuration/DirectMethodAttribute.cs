using System;

namespace ExtDirectHandler.Configuration
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DirectMethodAttribute : Attribute
	{
		public bool FormHandler { get; set; }
		public bool NamedArguments { get; set; }
	}
}