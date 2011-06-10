using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExtDirectHandler
{
	public class ObjectFactory
	{
		public virtual object GetInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public virtual JsonSerializer GetJsonSerializer()
		{
			return new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };
		}

		public virtual void Release(object instance) {}
	}
}