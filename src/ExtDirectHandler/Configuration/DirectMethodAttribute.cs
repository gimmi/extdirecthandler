using System;

namespace SpikeHttpHandler.Configuration
{
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