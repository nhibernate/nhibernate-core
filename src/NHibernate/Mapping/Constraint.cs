using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class for relational constraints in the database.
	/// </summary>
	[Serializable]
	public abstract class Constraint 
	{
		private string name;
		private readonly List<Column> columns = new List<Column>();
		private Table table;

		/// <summary>
		/// Gets or sets the Name used to identify the constraint in the database.
		/// </summary>
		/// <value>The Name used to identify the constraint in the database.</value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable{Column}"/> of <see cref="Column"/> objects that are part of the constraint.
		/// </summary>
		/// <value>
		/// An <see cref="IEnumerable{Column}"/> of <see cref="Column"/> objects that are part of the constraint.
		/// </value>
		public IEnumerable<Column> ColumnIterator
		{
			get { return columns; }
		}

		/// <summary>
		/// Adds the <see cref="Column"/> to the <see cref="ICollection"/> of 
		/// Columns that are part of the constraint.
		/// </summary>
		/// <param name="column">The <see cref="Column"/> to include in the Constraint.</param>
		public void AddColumn(Column column)
		{
			if (!columns.Contains(column))
				columns.Add(column);
		}

		public void AddColumns(IEnumerable<Column> columnIterator)
		{
			foreach (Column col in columnIterator)
			{
				if (!col.IsFormula)
					AddColumn(col);
			}
		}

		/// <summary>
		/// Gets the number of columns that this Constraint contains.
		/// </summary>
		/// <value>
		/// The number of columns that this Constraint contains.
		/// </value>
		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		public IList<Column> Columns
		{
			get{return columns;}
		}

		/// <summary>
		/// Gets or sets the <see cref="Table"/> this Constraint is in.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> this Constraint is in.
		/// </value>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

	

		public virtual bool IsGenerated(Dialect.Dialect dialect)
		{
			return true;
		}

		public override String ToString()
		{
			return string.Format("{0}({1}{2}) as {3}", GetType().FullName, Table.Name, StringHelper.CollectionToString(Columns), name);
		}
	}
}
