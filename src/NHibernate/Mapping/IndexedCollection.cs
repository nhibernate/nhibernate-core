using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Indexed collections include IList, IDictionary, Arrays
	/// and primitive Arrays.
	/// </summary>
	public abstract class IndexedCollection : Collection
	{
		/// <summary></summary>
		public const string DefaultIndexColumnName = "idx";

		private SimpleValue index;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected IndexedCollection( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public SimpleValue Index
		{
			get { return index; }
			set { index = value; }
		}

		/// <summary></summary>
		public override bool IsIndexed
		{
			get { return true; }
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

				// Index should be last column listed
				foreach( Column col in Index.ColumnCollection )
				{
					pk.AddColumn( col );
				}

				CollectionTable.PrimaryKey = pk;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		public override void Validate( IMapping mapping )
		{
			base.Validate( mapping );
			if ( !Index.IsValid( mapping ) )
			{
				throw new MappingException( string.Format( "collection index mapping has wrong number of columns: {0} type: {1}", Role, Index.Type.Name ) );
			}
		}
	}
}