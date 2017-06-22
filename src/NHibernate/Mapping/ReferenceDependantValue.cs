using System;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ReferenceDependantValue : DependantValue
	{
		private readonly SimpleValue _prototype;

		public ReferenceDependantValue(Table table, SimpleValue prototype)
			: base(table, prototype)
		{
			_prototype = prototype;
		}

		public IEnumerable<Column> ReferenceColumns
		{
			get { return _prototype.ConstraintColumns; }
		}

		public override void CreateForeignKeyOfEntity(string entityName)
		{
			if (!HasFormula && !string.Equals("none", ForeignKeyName, StringComparison.InvariantCultureIgnoreCase))
			{
				var referencedColumns = new List<Column>(_prototype.ColumnSpan);
				foreach (Column column in _prototype.ColumnIterator)
				{
					referencedColumns.Add(column);
				}

				ForeignKey fk = Table.CreateForeignKey(ForeignKeyName, ConstraintColumns, entityName, referencedColumns);
				fk.CascadeDeleteEnabled = IsCascadeDeleteEnabled;
			}
		}
	}
}
