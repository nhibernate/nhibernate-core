using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
		/// Generate a name hopefully unique using the table and column names.
		/// Static so the name can be generated prior to creating the Constraint.
		/// They're cached, keyed by name, in multiple locations.
		/// </summary>
		/// <param name="prefix">A name prefix for the generated name.</param>
		/// <param name="table">The table for which the name is generated.</param>
		/// <param name="referencedTable">The referenced table, if any.</param>
		/// <param name="columns">The columns for which the name is generated.</param>
		/// <returns>The generated name.</returns>
		/// <remarks>Hybrid of Hibernate <c>Constraint.generateName</c> and
		/// <c>NamingHelper.generateHashedFkName</c>.</remarks>
		public static string GenerateName(
			string prefix, Table table, Table referencedTable, IEnumerable<Column> columns)
		{
			// Use a concatenation that guarantees uniqueness, even if identical names
			// exist between all table and column identifiers.
			var sb = new StringBuilder("table`" + table.Name + "`");
			if (referencedTable != null)
				sb.Append("references`" + referencedTable.Name + "`");

			// Ensure a consistent ordering of columns, regardless of the order
			// they were bound.
			// Clone the list, as sometimes a set of order-dependent Column
			// bindings are given.
			var alphabeticalColumns = new List<Column>(columns);
			alphabeticalColumns.Sort(ColumnComparator.Instance);
			foreach (var column in alphabeticalColumns)
			{
				var columnName = column == null ? "" : column.Name;
				sb.Append("column`").Append(columnName).Append("`");
			}
			// Hash the generated name for avoiding collisions with user choosen names.
			// This is not 100% reliable, as hashing may still have a chance of generating
			// collisions.
			var name = prefix + HashName(sb.ToString());

			// Hibernate uses an algorithm yielding names shorter than 30 characters. But we cannot
			// use it (see HashName). And also we have DB limited to even less (Informix)...
			if (name.Length > 30)
			{
				// This, of course, increases the collision risk.
				name = name.Substring(0, 30);
			}

			return name;
		}

		#region Name generation support methods

		/// <summary>
		/// Hash a constraint name. Convert the hash digest to base 32
		/// (full alphanumeric) for shortening the hash string representation
		/// while keeping it suitable for db names.
		/// </summary>
		/// <param name="name">The name to be hashed.</param>
		/// <returns>The hased name.</returns>
		private static string HashName(string name)
		{
			// Hibernate uses MD5, but with .Net this would throw on FIPS enabled machine.
			// As a consequence generated names will be quite longer.
			using (var hasher = SHA256.Create())
			{
				var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(name));
				// Converting to base 32 for shortening the name.
				// Hibernate uses base 35, but we do not have a native implementation
				// in .Net, and base 32 is easier to implement.
				return ToBase32String(hash);
			}
		}

		// Adapted from https://stackoverflow.com/a/7135008/1178314
		// Changed for not padding with "="
		private static string ToBase32String(byte[] input)
		{
			if (input == null || input.Length == 0)
			{
				throw new ArgumentNullException(nameof(input));
			}

			var charCount = (int) Math.Ceiling(input.Length / 5d) * 8;
			var result = new StringBuilder(charCount);

			byte nextChar = 0, bitsRemaining = 5;

			foreach (var b in input)
			{
				nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
				result.Append(ValueToChar(nextChar));

				if (bitsRemaining < 4)
				{
					nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
					result.Append(ValueToChar(nextChar));
					bitsRemaining += 5;
				}

				bitsRemaining -= 3;
				nextChar = (byte)((b << bitsRemaining) & 31);
			}

			// If we didn't end with a full char
			if (result.Length != charCount)
			{
				result.Append(ValueToChar(nextChar));
			}

			return result.ToString();
		}

		private static char ValueToChar(byte b)
		{
			if (b < 26)
			{
				return (char)(b + 65);
			}

			if (b < 32)
			{
				return (char)(b + 24);
			}

			throw new ArgumentException("Byte is not a value Base32 value.", "b");
		}

		private class ColumnComparator : IComparer<Column>
		{
			public static readonly ColumnComparator Instance = new ColumnComparator();

			public int Compare(Column col1, Column col2) {
				return StringComparer.Ordinal.Compare(col1?.Name, col2?.Name);
			}
		}

		#endregion

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
			get { return columns; }
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
			if (!IsGenerated(dialect))
			{
				return null;
			}

			var catalog = Table.GetQuotedCatalog(dialect, defaultCatalog);
			var schema = Table.GetQuotedSchema(dialect, defaultSchema);
			var tableName = Table.GetQuotedName(dialect);

			return new StringBuilder()
						.AppendLine(dialect.GetIfExistsDropConstraint(catalog, schema, tableName, Name))
						.Append("alter table ")
						.Append(Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema))
						.Append(" drop constraint ")
						.AppendLine(Name)
						.Append(dialect.GetIfExistsDropConstraintEnd(catalog, schema, tableName, Name))
						.ToString();
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
			if (!IsGenerated(dialect))
			{
				return null;
			}

			StringBuilder buf = new StringBuilder("alter table ")
				.Append(Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema))
				.Append(SqlConstraintString(dialect, Name, defaultCatalog, defaultSchema));
			return buf.ToString();
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

		public override String ToString()
		{
			return string.Format("{0}({1}{2}) as {3}", GetType().FullName, Table.Name, StringHelper.CollectionToString(Columns), name);
		}
	}
}
