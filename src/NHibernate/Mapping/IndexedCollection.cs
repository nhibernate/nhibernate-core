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

		private Value index;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected IndexedCollection( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public Value Index
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
		public void CreatePrimaryKey()
		{
			PrimaryKey pk = new PrimaryKey();

			foreach( Column col in Key.ColumnCollection )
			{
				pk.AddColumn( col );
			}
			foreach( Column col in Index.ColumnCollection )
			{
				pk.AddColumn( col );
			}

			Table.PrimaryKey = pk;
		}
	}
}