using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Index in the database.
	/// </summary>
	public class Index : IRelationalModel
	{
		private Table table;
		private ArrayList columns = new ArrayList();
		private string name;

		/// <summary>
		/// Generates the SQL string to create this Index in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create this Index.
		/// </returns>
		public string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultSchema)
		{
			StringBuilder buf =
				new StringBuilder("create index ")
				.Append(dialect.QualifyIndexName ? name : StringHelper.Unqualify(name))
				.Append(" on ")
				.Append(table.GetQualifiedName(dialect, defaultSchema))
				.Append(" (");

			bool commaNeeded = false;
			for (int i = 0; i < columns.Count; i++)
			{
				if (commaNeeded)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				commaNeeded = true;

				buf.Append(((Column) columns[i]).GetQuotedName(dialect));
			}

			buf.Append(StringHelper.ClosedParen);

			string ifExists = dialect.GetIfNotExistsCreateConstraint(Table, Name);
			string create = buf.ToString();
			string end = dialect.GetIfNotExistsCreateConstraintEnd(Table, Name);

			return ifExists + System.Environment.NewLine + create + System.Environment.NewLine + end;
		}

		/// <summary>
		/// Generates the SQL string to drop this Index in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Index.
		/// </returns>
		public string SqlDropString(Dialect.Dialect dialect, string defaultSchema)
		{
			string ifExists = dialect.GetIfExistsDropConstraint(Table, Name);
			string drop = string.Format("drop index {0}.{1}", table.GetQualifiedName(dialect, defaultSchema), name);
			string end = dialect.GetIfExistsDropConstraintEnd(Table, Name);
			return ifExists + System.Environment.NewLine + drop + System.Environment.NewLine + end;
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
		public ICollection ColumnCollection
		{
			get { return columns; }
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

		public void AddColumns(IEnumerable extraColumns)
		{
			foreach (Column column in extraColumns)
			{
				AddColumn(column);
			}
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
	}
}