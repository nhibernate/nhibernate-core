using System;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A collection with a synthetic "identifier" column.
	/// </summary>
	public abstract class IdentifierCollection : Collection
	{
		public static readonly string DefaultIdentifierColumnName = "id";
		private Value identifier;

		protected IdentifierCollection(PersistentClass owner) : base(owner)
		{
		}

		public Value Identifier 
		{
			get { return identifier; }
			set { identifier = value; }
		}

		public override bool IsIdentified
		{
			get { return true; }
		}

		public void CreatePrimaryKey() 
		{
			PrimaryKey pk = new PrimaryKey();
			foreach(Column col in Identifier.ColumnCollection) 
			{
				pk.AddColumn(col);
			}

			Table.PrimaryKey = pk;
		}

	}
}
