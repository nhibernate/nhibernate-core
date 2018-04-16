using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Primary Key constraint in the database.
	/// </summary>
	[Serializable]
	public class PrimaryKey : Constraint
	{
		/// <summary>
		/// Generates the SQL string to create the Primary Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the Primary Key Constraint.
		/// </returns>
		public string SqlConstraintString(Dialect.Dialect d, string defaultSchema)
		{
			StringBuilder buf = new StringBuilder(d.PrimaryKeyString + " (");
			int i = 0;
			foreach (Column col in ColumnIterator)
			{
				buf.Append(col.GetQuotedName(d));
				if (i < ColumnSpan - 1)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				i++;
			}
			return buf.Append(StringHelper.ClosedParen).ToString();
		}

		/// <summary>
		/// Generates the SQL string to create the named Primary Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Primary Key Constraint.
		/// </returns>
		public override string SqlConstraintString(Dialect.Dialect d, string constraintName, string defaultCatalog, string defaultSchema)
		{
			StringBuilder buf = new StringBuilder(
				d.GetAddPrimaryKeyConstraintString(constraintName))
				.Append('(');
			int i = 0;
			foreach (Column col in ColumnIterator)
			{
				buf.Append(col.GetQuotedName(d));
				if (i < ColumnSpan - 1)
				{
					buf.Append(StringHelper.CommaSpace);
				}
				i++;
			}
			return buf.Append(StringHelper.ClosedParen).ToString();
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
			var catalog = Table.GetQuotedCatalog(dialect, defaultCatalog);
			var schema = Table.GetQuotedSchema(dialect, defaultSchema);
			var tableName = Table.GetQuotedName(dialect);

			return new StringBuilder()
							.AppendLine(dialect.GetIfExistsDropConstraint(catalog, schema, tableName, Name))
							.Append("alter table ")
							.Append(Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema))
							.AppendLine(dialect.GetDropPrimaryKeyConstraintString(Name))
							.Append(dialect.GetIfExistsDropConstraintEnd(catalog, schema, tableName, Name))
							.ToString();
		}

		#endregion
	}
}
