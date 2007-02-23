using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A list has a primary key consisting of the key columns + index column
	/// </summary>
	public class List : IndexedCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="List"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this list mapping.</param>
		public List(PersistentClass owner) : base(owner)
		{
		}

		/// <summary>
		/// Gets the appropriate <see cref="CollectionType"/> that is 
		/// specialized for this list mapping.
		/// </summary>
		public override CollectionType DefaultCollectionType
		{
			get
			{
#if NET_2_0
				if (this.IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					return TypeFactory.GenericList(Role, ReferencedPropertyName, this.GenericArguments[0]);
				}
#endif
				return TypeFactory.List(Role, ReferencedPropertyName);
			}
		}
	}
}