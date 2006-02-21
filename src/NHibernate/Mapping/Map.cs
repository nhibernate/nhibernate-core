using System;

using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A map has a primary key consisting of the key columns 
	/// + index columns.
	/// </summary>
	public class Map : IndexedCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Map"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this map mapping.</param>
		public Map(PersistentClass owner)
			: base(owner)
		{
		}

		/// <summary>
		/// Gets the appropriate <see cref="PersistentCollectionType"/> that is 
		/// specialized for this list mapping.
		/// </summary>
		public override PersistentCollectionType CollectionType
		{
			get
			{
#if NET_2_0
				if (this.IsGeneric)
				{
					return TypeFactory.GenericMap( Role, ReferencedPropertyName, this.GenericArguments[0], this.GenericArguments[1] );
					// TODO: deal with sorting
				}
				else
				{
					return IsSorted ? 
							TypeFactory.SortedMap( Role, ReferencedPropertyName, Comparer ) : 
							TypeFactory.Map( Role, ReferencedPropertyName );
				}
#else

				return IsSorted ?
					TypeFactory.SortedMap( Role, ReferencedPropertyName, Comparer ) :
					TypeFactory.Map( Role, ReferencedPropertyName );
#endif
			}
		}

		/// <summary></summary>
		public override void CreateAllKeys( )
		{
			base.CreateAllKeys();
			if ( !IsInverse )
			{
				Index.CreateForeignKey();
			}
		}
	}
}