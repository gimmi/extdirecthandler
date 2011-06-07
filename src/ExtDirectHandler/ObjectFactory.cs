using System;

namespace ExtDirectHandler
{
	public class ObjectFactory
	{
		public virtual object GetInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public virtual void Release(object instance) { }
	}
}