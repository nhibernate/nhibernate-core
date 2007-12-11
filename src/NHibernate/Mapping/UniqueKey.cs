using System.Text;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Unique Key constraint in the database.
	/// </summary>
	public class UniqueKey : Constraint
	{
		/// <summary>
		/// Generates the SQL string to create the Unique Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <returns>
		/// A string that contains the SQL to create the Unique Key Constraint.
		/// </returns>
		public string SqlConstraintString(Dialect.Dialect d)
		{
			StringBuilder buf = new StringBuilder(" unique (");
			bool commaNeeded = false;

			foreach (Column col in ColumnIterator)
			{
				if (commaNeeded)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				commaNeeded = true;

				buf.Append(col.GetQuotedName(d));
			}

			return buf.Append(StringHelper.ClosedParen).ToString();
		}

		/// <summary>
		/// Generates the SQL string to create the Unique Key Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName"></param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the Unique Key Constraint.
		/// </returns>
		public override string SqlConstraintString(Dialect.Dialect dialect, string constraintName, string defaultCatalog, string defaultSchema)
		{
			StringBuilder buf = new StringBuilder(
				dialect.GetAddPrimaryKeyConstraintString(constraintName))
				.Append('(');

			bool commaNeeded = false;

			foreach (Column col in ColumnIterator)
			{
				if (commaNeeded)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				commaNeeded = true;

				buf.Append(col.GetQuotedName(dialect));
			}

			string ifExists = dialect.GetIfNotExistsCreateConstraint(Table, Name);
			string create = StringHelper.Replace(buf.Append(StringHelper.ClosedParen).ToString(), "primary key", "unique");
			string end = dialect.GetIfNotExistsCreateConstraintEnd(Table, Name);

			return ifExists + System.Environment.NewLine + create + System.Environment.NewLine + end;
		}

		#region IRelationalModel Members

		/// <summary>
		/// Get the SQL string to drop this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Constraint.
		/// </returns>
		public override string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			string ifExists = dialect.GetIfExistsDropConstraint(Table, Name);
			string drop = "alter table " + Table.GetQualifiedName(dialect, defaultSchema) + dialect.GetDropIndexConstraintString(Name);
			string end = dialect.GetIfExistsDropConstraintEnd(Table, Name);

			return ifExists + System.Environment.NewLine + drop + System.Environment.NewLine + end;
		}

		#endregion

		public override bool IsGenerated(Dialect.Dialect dialect)
		{
			if (dialect.SupportsNotNullUnique)
				return true;
			foreach (Column column in ColumnIterator)
			{
				if(column.IsNullable)
					return false;
			}
			return true;
		}
	}
}