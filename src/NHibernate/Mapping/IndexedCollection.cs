using System;

namespace NHibernate.Mapping 
{
	/// <summary>
	/// Indexed collections include IList, IDictionary, Arrays
	/// and primitive Arrays.
	/// </summary>
	public abstract class IndexedCollection : Collection	
	{
		public const string DefaultIndexColumnName = "idx";

		private Value index;

		protected IndexedCollection(PersistentClass owner) : base(owner) 
		{ 
		}

		public Value Index 
		{
			get { return index; }
			set { index = value; }
		}

		public override bool IsIndexed 
		{
			get { return true; }
		}

		public void CreatePrimaryKey() 
		{
			PrimaryKey pk = new PrimaryKey();
			
			foreach(Column col in Key.ColumnCollection ) 
			{
				pk.AddColumn(col);
			}
			foreach(Column col in Index.ColumnCollection) 
			{
				pk.AddColumn(col);
			}

			Table.PrimaryKey = pk;
		}
	}
}
