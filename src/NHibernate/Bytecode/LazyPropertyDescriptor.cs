using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Type;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Descriptor for a property which is enabled for bytecode lazy fetching
	///
	/// Author: Steve Ebersole
	/// </summary>
	[Serializable]
	public class LazyPropertyDescriptor
	{
		public static LazyPropertyDescriptor From(
			Mapping.Property property,
			int propertyIndex,
			int lazyIndex)
		{
			var fetchGroupName = string.IsNullOrEmpty(property.LazyGroup)
				? "DEFAULT"
				: property.LazyGroup;

			return new LazyPropertyDescriptor(
				propertyIndex,
				lazyIndex,
				property.Name,
				property.Type,
				fetchGroupName
			);
		}

		private LazyPropertyDescriptor(
			int propertyIndex,
			int lazyIndex,
			string name,
			IType type,
			string fetchGroupName)
		{
			if (propertyIndex < lazyIndex)
			{
				throw new InvalidOperationException("Property index is lower than the lazy index.");
			}

			PropertyIndex = propertyIndex;
			LazyIndex = lazyIndex;
			Name = name;
			Type = type;
			FetchGroupName = fetchGroupName;
		}

		/// <summary>
		/// Access to the index of the property in terms of its position in the entity persister
		/// </summary>
		public int PropertyIndex { get; }

		/// <summary>
		/// Access to the index of the property in terms of its position within the lazy properties of the persister
		/// </summary>
		public int LazyIndex { get; }

		/// <summary>
		/// Access to the name of the property
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Access to the property's type
		/// </summary>
		public IType Type { get; }

		/// <summary>
		/// Access to the name of the fetch group to which the property belongs
		/// </summary>
		public string FetchGroupName { get; }
	}
}
