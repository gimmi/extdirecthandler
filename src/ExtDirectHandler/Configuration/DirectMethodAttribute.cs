using System;

namespace ExtDirectHandler.Configuration
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DirectMethodAttribute : Attribute
	{
		private readonly string _name;

		public DirectMethodAttribute(string name)
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}
	}
}