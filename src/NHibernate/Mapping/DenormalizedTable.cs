using System.Collections;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	public class DenormalizedTable: Table
	{
		private readonly Table includedTable;

		public DenormalizedTable(Table includedTable)
		{
			this.includedTable = includedTable;
			includedTable.SetHasDenormalizedTables();
		}

		public override ICollection ColumnCollection
		{
			get
			{
				ArrayList result = new ArrayList(includedTable.ColumnCollection);
				result.AddRange(base.ColumnCollection);
				return result;
			}
		}

		public override ICollection UniqueKeyCollection
		{
			get
			{
				//wierd implementation because of hacky behavior
				//of Table.SqlCreateString() which modifies the
				//list of unique keys by side-effect on some dialects
				IDictionary uks = new Hashtable();
				ArrayHelper.AddAll(uks, UniqueKeys);
				ArrayHelper.AddAll(uks, includedTable.UniqueKeys);
				return uks.Values;
			}
		}

		public override ICollection IndexCollection
		{
			get
			{
				ArrayList indexes = new ArrayList();
				ICollection includedIdxs = includedTable.IndexCollection;
				foreach (Index parentIndex in includedIdxs)
				{
					Index index = new Index();
					index.Name = Name + parentIndex.Name;
					index.Table = this;
					index.AddColumns(parentIndex.ColumnCollection);
					indexes.Add(index);
				}
				indexes.AddRange(base.IndexCollection);
				return indexes;
			}
		}

		public override void CreateForeignKeys()
		{
			includedTable.CreateForeignKeys();
			ICollection includedFks = includedTable.ForeignKeyCollection;
			foreach (ForeignKey fk in includedFks)
			{
				// NH Different behaviour 
				CreateForeignKey(GetForeignKeyName(fk), fk.Columns, fk.ReferencedClass);
			}
		}

		private string GetForeignKeyName(ForeignKey fk)
		{
			// (the FKName length, of H3.2 implementation, may be to long for some RDBMS so we implement something different) 
			int hash = fk.Name.GetHashCode() ^ Name.GetHashCode();
			return string.Format("KF{0}", hash.ToString("X"));
		}

		public override Column GetColumn(Column column)
		{
			Column superColumn = base.GetColumn(column);
			if (superColumn != null)
			{
				return superColumn;
			}
			else
			{
				return includedTable.GetColumn(column);
			}
		}

		public override bool ContainsColumn(Column column)
		{
			return base.ContainsColumn(column) || includedTable.ContainsColumn(column);
		}

		public override PrimaryKey PrimaryKey
		{
			get
			{
				return includedTable.PrimaryKey;
			}
			set
			{
				base.PrimaryKey = value;
			}
		}
	}
}