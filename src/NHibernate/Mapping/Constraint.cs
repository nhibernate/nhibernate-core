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
	public abstract class Constraint : IRelationalModel
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

		#region IRelationModel Members

		/// <summary>
		/// Generates the SQL string to drop this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <param name="defaultCatalog"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Constraint.
		/// </returns>
		public virtual string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			if (IsGenerated(dialect))
			{
				string ifExists = dialect.GetIfExistsDropConstraint(Table, Name);
				string drop =
					string.Format("alter table {0} drop constraint {1}", Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema), Name);
				string end = dialect.GetIfExistsDropConstraintEnd(Table, Name);

				return ifExists + System.Environment.NewLine + drop + System.Environment.NewLine + end;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Generates the SQL string to create this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultSchema"></param>
		/// <param name="defaultCatalog"></param>
		/// <returns>
		/// A string that contains the SQL to create this Constraint.
		/// </returns>
		public virtual string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			if (IsGenerated(dialect))
			{
				string constraintString = SqlConstraintString(dialect, Name, defaultCatalog, defaultSchema);
				StringBuilder buf = new StringBuilder("alter table ")
					.Append(Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema))
					.Append(constraintString);
				return buf.ToString();
			}
			else
			{
				return null;
			}
		}

		#endregion

		/// <summary>
		/// When implemented by a class, generates the SQL string to create the named
		/// Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultSchema"></param>
		/// <param name="defaultCatalog"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Constraint.
		/// </returns>
		public abstract string SqlConstraintString(Dialect.Dialect d, string constraintName, string defaultCatalog, string defaultSchema);

		public virtual bool IsGenerated(Dialect.Dialect dialect)
		{
			return true;
		}

		public override System.String ToString()
		{
			return
				string.Format("{0}({1}{2}) as {3}", GetType().FullName, Table.Name,
				              StringHelper.CollectionToString((ICollection) Columns), name);
		}
	}
}
