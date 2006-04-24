using System;

using NHibernate.Type;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A map has a primary key consisting of the key columns 
	/// + index columns.
	/// </summary>
	public class Map : IndexedCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PersistentMap"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this map mapping.</param>
		public Map( PersistentClass owner ) : base( owner )
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
				if( this.IsGeneric )
				{
					if( HasOrder )
					{
						throw new MappingException( "Cannot use order-by with generic map, no appropriate collection implementation is available" );
					}
					else if( IsSorted )
					{
						throw new MappingException( "Use collection-type='sorted-list/sorted-dictionary' to choose implementation for generic map" );
					}
					else
					{
						return TypeFactory.GenericMap( Role, ReferencedPropertyName, this.GenericArguments[ 0 ], this.GenericArguments[ 1 ] );
					}
				}
#endif
				if( HasOrder )
				{
					return TypeFactory.OrderedMap( Role, ReferencedPropertyName );
				}
				else if( IsSorted )
				{
					return TypeFactory.SortedMap( Role, ReferencedPropertyName, ( IComparer ) Comparer );
				}
				else
				{
					return TypeFactory.Map( Role, ReferencedPropertyName );
				}
			}
		}

		public override void CreateAllKeys()
		{
			base.CreateAllKeys();
			if( !IsInverse )
			{
				Index.CreateForeignKey();
			}
		}
	}
}