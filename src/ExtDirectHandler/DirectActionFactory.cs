using System;

namespace SpikeHttpHandler
{
	public class DirectActionFactory
	{
		private object GetInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		private void Release(object instance) {}
	}
}