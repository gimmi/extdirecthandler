using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpikeHttpHandler.Configuration
{
	public class AttributeConfigurator
	{
		private readonly IList<Type> _types = new List<Type>();

		public void AddTypes(Assembly assembly)
		{
			AddTypes(assembly.GetTypes());
		}

		public void AddTypes(IEnumerable<Type> types)
		{
			foreach(Type type in types.Where(HasAttribute<DirectActionAttribute>))
			{
				AddType(type);
			}
		}

		public void AddType(Type type)
		{
			_types.Add(type);
		}

		public void Configure()
		{
			Dictionary<string, DirectActionMetadata> actions = _types.ToDictionary(
				t => GetAttribute<DirectActionAttribute>(t).Name,
				t => new DirectActionMetadata{ Type = t, Methods = FindDirectMethods(t) }
				);
			DirectHttpHandler.SetActionMetadatas(actions);
		}

		private Dictionary<string, DirectMethodMetadata> FindDirectMethods(Type type)
		{
			return type.GetMethods()
				.Where(HasAttribute<DirectMethodAttribute>)
				.ToDictionary(
					m => GetAttribute<DirectMethodAttribute>(m).Name,
					m => new DirectMethodMetadata{ Method = m }
				);
		}

		private bool HasAttribute<T>(MemberInfo member)
		{
			return member.GetCustomAttributes(typeof(T), false).Count() > 0;
		}

		private T GetAttribute<T>(MemberInfo member)
		{
			object[] attributes = member.GetCustomAttributes(typeof(T), false);
			if(attributes.Length == 0)
			{
				throw new InvalidOperationException(string.Format("Member {0} must be decorated with {1}", member, typeof(T)));
			}
			return (T)attributes.FirstOrDefault();
		}
	}
}