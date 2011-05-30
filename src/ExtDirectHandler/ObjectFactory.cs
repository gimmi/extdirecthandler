using System;

namespace ExtDirectHandler
{
	public class ObjectFactory
	{
		public object GetInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public void Release(object instance) {}
	}
}