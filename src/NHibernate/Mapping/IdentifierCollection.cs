namespace NHibernate.Mapping
{
	/// <summary>
	/// A collection with a synthetic "identifier" column.
	/// </summary>
	public abstract class IdentifierCollection : Collection
	{
		/// <summary></summary>
		public static readonly string DefaultIdentifierColumnName = "id";
		private Value identifier;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		protected IdentifierCollection( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public Value Identifier
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
		public void CreatePrimaryKey()
		{
			PrimaryKey pk = new PrimaryKey();
			foreach( Column col in Identifier.ColumnCollection )
			{
				pk.AddColumn( col );
			}

			Table.PrimaryKey = pk;
		}

	}
}