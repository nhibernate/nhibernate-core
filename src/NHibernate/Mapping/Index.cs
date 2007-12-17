using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Index in the database.
	/// </summary>
	[Serializable]
	public class Index : IRelationalModel
	{
		private Table table;
		private readonly List<Column> columns = new List<Column>();
		private string name;

		public static string BuildSqlCreateIndexString(Dialect.Dialect dialect, string name, Table table,
			IEnumerable<Column> columns, bool unique, string defaultCatalog, string defaultSchema)
		{
			//TODO handle supportsNotNullUnique=false, but such a case does not exist in the wild so far
			StringBuilder buf = new StringBuilder("create")
				.Append(unique ? " unique" : "")
				.Append(" index ")
				.Append(dialect.QualifyIndexName ? name : StringHelper.Unqualify(name))
				.Append(" on ")
				.Append(table.GetQualifiedName(dialect, defaultCatalog, defaultSchema))
				.Append(" (");
			bool commaNeeded = false;
			foreach (Column column in columns)
			{
				if (commaNeeded)
					buf.Append(StringHelper.CommaSpace);
				commaNeeded = true;

				buf.Append(column.GetQuotedName(dialect));
			}
			buf.Append(")");

			return buf.ToString();
		}

		public static string BuildSqlDropIndexString(Dialect.Dialect dialect, Table table, string name, string defaultCatalog, string defaultSchema)
		{
			string ifExists = dialect.GetIfExistsDropConstraint(table, name);
			string drop = string.Format("drop index {0}", StringHelper.Qualify(table.GetQualifiedName(dialect, defaultCatalog, defaultSchema), name));
			string end = dialect.GetIfExistsDropConstraintEnd(table, name);
			return ifExists + Environment.NewLine + drop + Environment.NewLine + end;
		}

		/// <summary>
		/// Generates the SQL string to create this Index in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create this Index.
		/// </returns>
		public string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			return BuildSqlCreateIndexString(dialect, Name, Table, ColumnIterator, false, defaultCatalog, defaultSchema);
		}

		/// <summary>
		/// Generates the SQL string to drop this Index in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Index.
		/// </returns>
		public string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			return BuildSqlDropIndexString(dialect, Table, Name, defaultCatalog, defaultSchema);
		}

		/// <summary>
		/// Gets or sets the <see cref="Table"/> this Index is in.
		/// </summary>
		/// <value>
		/// The <see cref="Table"/> this Index is in.
		/// </value>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Column"/> objects that are 
		/// part of the Index.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Column"/> objects that are 
		/// part of the Index.
		/// </value>
		public IEnumerable<Column> ColumnIterator
		{
			get { return columns; }
		}

		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary>
		/// Adds the <see cref="Column"/> to the <see cref="ICollection"/> of 
		/// Columns that are part of the Index.
		/// </summary>
		/// <param name="column">The <see cref="Column"/> to include in the Index.</param>
		public void AddColumn(Column column)
		{
			if (!columns.Contains(column))
			{
				columns.Add(column);
			}
		}

		public void AddColumns(IEnumerable<Column> extraColumns)
		{
			foreach (Column column in extraColumns)
				AddColumn(column);
		}

		/// <summary>
		/// Gets or sets the Name used to identify the Index in the database.
		/// </summary>
		/// <value>The Name used to identify the Index in the database.</value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool ContainsColumn(Column column)
		{
			return columns.Contains(column);
		}

		public override string ToString()
		{
			return GetType().FullName + "(" + Name + ")";
		}
	}
}