using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtDirectHandler.Configuration
{
	public class AttributeConfigurator
	{
		private readonly IList<Type> _types = new List<Type>();

		public AttributeConfigurator AddTypes(Assembly assembly)
		{
			AddTypes(assembly.GetTypes());
			return this;
		}

		public AttributeConfigurator AddTypes(IEnumerable<Type> types)
		{
			foreach(Type type in types.Where(HasAttribute<DirectActionAttribute>))
			{
				AddType(type);
			}
			return this;
		}

		public AttributeConfigurator AddType(Type type)
		{
			_types.Add(type);
			return this;
		}

		public void Configure()
		{
			Dictionary<string, DirectActionMetadata> actions = _types.Select(t => new{ GetAttribute<DirectActionAttribute>(t).Name, Type = t, Methods = FindDirectMethods(t) }).ToDictionary(
				x => x.Name,
				x => new DirectActionMetadata{ Name = x.Name, Type = x.Type, Methods = x.Methods }
				);
			DirectHttpHandler.SetActionMetadatas(actions);
		}

		private Dictionary<string, DirectMethodMetadata> FindDirectMethods(Type type)
		{
			return type.GetMethods()
				.Where(HasAttribute<DirectMethodAttribute>)
				.Select(m => new { GetAttribute<DirectMethodAttribute>(m).Name, Method = m })
				.ToDictionary(
					x => x.Name,
					x => new DirectMethodMetadata{ Name = x.Name, Method = x.Method }
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