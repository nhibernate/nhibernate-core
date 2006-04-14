using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A bag permits duplicates, so it has no primary key
	/// </summary>
	public class Bag : Collection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PersistentBag"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this bag mapping.</param>
		public Bag( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary>
		/// Gets the appropriate <see cref="CollectionType"/> that is 
		/// specialized for this bag mapping.
		/// </summary>
		public override CollectionType CollectionType
		{
			get
			{
#if NET_2_0
				if (this.IsGeneric)
				{
					return TypeFactory.GenericBag( Role, ReferencedPropertyName, this.GenericArguments[0] );
				}
				else
				{
					return TypeFactory.Bag( Role, ReferencedPropertyName );
				}
#else
				return TypeFactory.Bag( Role, ReferencedPropertyName );
#endif
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Should we create an index on the key columns?</remarks>
		public override void CreatePrimaryKey( )
		{
		}
	}
}