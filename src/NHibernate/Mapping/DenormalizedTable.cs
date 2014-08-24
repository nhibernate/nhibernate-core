using System;
using System.Collections;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	[Serializable]
	public class DenormalizedTable : Table
	{
		private readonly Table includedTable;

		public DenormalizedTable(Table includedTable)
		{
			this.includedTable = includedTable;
			includedTable.SetHasDenormalizedTables();
		}

		public override IEnumerable<Column> ColumnIterator
		{
			get { return new JoinedEnumerable<Column>(includedTable.ColumnIterator, base.ColumnIterator); }
		}

		public override IEnumerable<UniqueKey> UniqueKeyIterator
		{
			get
			{
				//weird implementation because of hacky behavior
				//of Table.SqlCreateString() which modifies the
				//list of unique keys by side-effect on some dialects
				Dictionary<string, UniqueKey> uks = new Dictionary<string, UniqueKey>();
				ArrayHelper.AddAll(uks, UniqueKeys);
				ArrayHelper.AddAll(uks, includedTable.UniqueKeys);
				return uks.Values;
			}
		}

		public override IEnumerable<Index> IndexIterator
		{
			get
			{
				List<Index> indexes = new List<Index>();
				IEnumerable<Index> includedIdxs = includedTable.IndexIterator;
				foreach (Index parentIndex in includedIdxs)
				{
					Index index = new Index();
					index.Name = Name + parentIndex.Name;
					index.Table = this;
					index.AddColumns(parentIndex.ColumnIterator);
					indexes.Add(index);
				}
				return new JoinedEnumerable<Index>(indexes, base.IndexIterator);
			}
		}

		public override void CreateForeignKeys()
		{
			includedTable.CreateForeignKeys();
			IEnumerable includedFks = includedTable.ForeignKeyIterator;
			foreach (ForeignKey fk in includedFks)
			{
				// NH Different behaviour (fk name)
				CreateForeignKey(GetForeignKeyName(fk), fk.Columns, fk.ReferencedEntityName);
			}
		}

		private string GetForeignKeyName(ForeignKey fk)
		{
			// (the FKName length, of H3.2 implementation, may be too long for some RDBMS so we implement something different) 
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
