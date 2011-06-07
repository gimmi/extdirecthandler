using System;

namespace ExtDirectHandler.Configuration
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DirectActionAttribute : Attribute
	{
		private readonly string _name;

		public DirectActionAttribute(string name)
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}
	}
}