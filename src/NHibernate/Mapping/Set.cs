using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Set with no nullable element columns will have a primary
	/// key consisting of all table columns (ie - key columns + 
	/// element columns).
	/// </summary>
	public class Set : Collection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Set( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary>
		/// <see cref="Collection.IsSet"/>
		/// </summary>
		public override bool IsSet
		{
			get { return true; }
		}

		/// <summary>
		/// <see cref="Collection.Type"/>
		/// </summary>
		public override PersistentCollectionType CollectionType
		{
			get
			{
				return IsSorted ?
					TypeFactory.SortedSet( Role, Comparer ) :
					TypeFactory.Set( Role );
			}
		}

		/// <summary></summary>
		public override void CreatePrimaryKey()
		{
			if ( !IsOneToMany )
			{
				PrimaryKey pk = new PrimaryKey();
				foreach( Column col in Key.ColumnCollection )
				{
					pk.AddColumn( col );
				}

				bool nullable = false;
				foreach( Column col in Element.ColumnCollection )
				{
					if( col.IsNullable )
					{
						nullable = true;
					}
					pk.AddColumn( col );
				}

				// some databases (Postgres) will tolerate nullable
				// column in a primary key - others (DB2) won't
				if( !nullable )
				{
					CollectionTable.PrimaryKey = pk;
				}
			}
			else
			{
				// Create an index on the key columns?
			}
		}
	}
}