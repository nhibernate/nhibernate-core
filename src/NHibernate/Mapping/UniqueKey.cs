using System;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An Unique Key constraint in the database.
	/// </summary>
	[Serializable]
	public class UniqueKey : Constraint
	{
		/// <summary>
		/// Generates the SQL string to create the Unique Key Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <returns> A string that contains the SQL to create the Unique Key Constraint. </returns>
		public string SqlConstraintString(Dialect.Dialect dialect)
		{
			StringBuilder buf = new StringBuilder("unique (");
			bool commaNeeded = false;
			bool nullable = false;
			foreach (Column column in ColumnIterator)
			{
				if (!nullable && column.IsNullable)
					nullable = true;

				if (commaNeeded)
					buf.Append(StringHelper.CommaSpace);
				commaNeeded = true;
				buf.Append(column.GetQuotedName(dialect));
			}
			//do not add unique constraint on DB not supporting unique and nullable columns
			return !nullable || dialect.SupportsNotNullUnique ? buf.Append(StringHelper.ClosedParen).ToString() : null;
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
			StringBuilder buf = new StringBuilder(dialect.GetAddPrimaryKeyConstraintString(constraintName))
				.Append(StringHelper.OpenParen);

			bool commaNeeded = false;
			bool nullable = false;
			foreach (Column column in ColumnIterator)
			{
				if (!nullable && column.IsNullable)
					nullable = true;
				if (commaNeeded)
					buf.Append(StringHelper.CommaSpace);
				commaNeeded = true;
				buf.Append(column.GetQuotedName(dialect));
			}

			return
				!nullable || dialect.SupportsNotNullUnique
					? StringHelper.Replace(buf.Append(StringHelper.ClosedParen).ToString(), "primary key", "unique")
					: null;
		}

		public override string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			if (dialect.SupportsUniqueConstraintInCreateAlterTable)
			{
				return base.SqlCreateString(dialect, p, defaultCatalog, defaultSchema);
			}
			else
			{
				return Index.BuildSqlCreateIndexString(dialect, Name, Table, ColumnIterator, true, defaultCatalog, defaultSchema);
			}
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
			if (dialect.SupportsUniqueConstraintInCreateAlterTable)
				return base.SqlDropString(dialect, defaultCatalog, defaultSchema);
			else
				return Index.BuildSqlDropIndexString(dialect, Table, Name, defaultCatalog, defaultSchema);
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