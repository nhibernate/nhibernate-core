using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Type;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Descriptor for a property which is mapped as lazy="no-proxy"
	/// </summary>
	public class UnwrapProxyPropertyDescriptor
	{
		public static UnwrapProxyPropertyDescriptor From(
			Mapping.Property property,
			int propertyIndex)
		{
			return new UnwrapProxyPropertyDescriptor(
				propertyIndex,
				property.Name,
				property.Type
			);
		}

		private UnwrapProxyPropertyDescriptor(
			int propertyIndex,
			string name,
			IType type)
		{
			PropertyIndex = propertyIndex;
			Name = name;
			Type = type;
		}

		/// <summary>
		/// Access to the index of the property in terms of its position in the entity persister
		/// </summary>
		public int PropertyIndex { get; }

		/// <summary>
		/// Access to the name of the property
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Access to the property's type
		/// </summary>
		public IType Type { get; set; }
	}
}
