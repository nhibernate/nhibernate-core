using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A collection with a synthetic "identifier" column.
	/// </summary>
	public abstract class IdentifierCollection : Collection
	{
		/// <summary></summary>
		public static readonly string DefaultIdentifierColumnName = "id";
		private SimpleValue identifier;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected IdentifierCollection( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public SimpleValue Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}

		/// <summary></summary>
		public override bool IsIdentified
		{
			get { return true; }
		}

		/// <summary></summary>
		public override void CreatePrimaryKey()
		{
			if ( !IsOneToMany )
			{
				PrimaryKey pk = new PrimaryKey();
				foreach( Column col in Identifier.ColumnCollection )
				{
					pk.AddColumn( col );
				}
				CollectionTable.PrimaryKey = pk;
			}
			//else  // Create an index on the key columns?
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public override void Validate( IMapping mapping )
		{
			base.Validate( mapping );
			if ( !Identifier.IsValid( mapping ) )
			{
				throw new MappingException( string.Format( "collection id mapping has wrong number of columns: {0} type: {1}", Role, Identifier.Type.Name ) );
			}
		}
	}
}