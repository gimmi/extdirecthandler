using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class ReflectionConfigurator
	{
		private readonly IList<Type> _types = new List<Type>();

		public ReflectionConfigurator RegisterTypes(params Type[] types)
		{
			return RegisterTypes(types.AsEnumerable());
		}

		public ReflectionConfigurator RegisterType<T>()
		{
			return RegisterType(typeof(T));
		}

		public ReflectionConfigurator RegisterTypes(IEnumerable<Type> types)
		{
			foreach(Type type in types)
			{
				RegisterType(type);
			}
			return this;
		}

		public ReflectionConfigurator RegisterType(Type type)
		{
			_types.Add(type);
			return this;
		}

		public void Configure()
		{
			DirectHttpHandler.SetActionMetadatas(BuildMetadata());
		}

		internal Dictionary<string, DirectActionMetadata> BuildMetadata()
		{
			Dictionary<string, DirectActionMetadata> actions = _types.Select(t => new{ t.Name, Type = t, Methods = FindDirectMethods(t) }).ToDictionary(
				x => x.Name,
				x => new DirectActionMetadata{ Type = x.Type, Methods = x.Methods }
				);
			return actions;
		}

		private Dictionary<string, MethodInfo> FindDirectMethods(Type type)
		{
			return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
				.Select(m => new{ Name = pascalizeName(m.Name), Method = m })
				.ToDictionary(
					x => x.Name,
					x => x.Method
				);
		}

		private string pascalizeName(string name)
		{
			return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
		}
	}
}