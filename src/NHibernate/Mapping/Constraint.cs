using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class for relational constraints in the database.
	/// </summary>
	public abstract class Constraint : IRelationalModel
	{
		private string name;
		private ArrayList columns = new ArrayList();
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
		/// Gets an <see cref="ICollection"/> of <see cref="Column"/> objects that are part of the constraint.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Column"/> objects that are part of the constraint.
		/// </value>
		public ICollection ColumnCollection
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
			{
				columns.Add(column);
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
		/// <returns>
		/// A string that contains the SQL to drop this Constraint.
		/// </returns>
		public virtual string SqlDropString(Dialect.Dialect dialect, string defaultSchema)
		{
			// TODO: NH-421
			return string.Format("alter table {0} drop constraint {1}", Table.GetQualifiedName(dialect, defaultSchema), Name);
		}

		/// <summary>
		/// Generates the SQL string to create this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create this Constraint.
		/// </returns>
		public string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultSchema)
		{
			// TODO: NH-421
			return
				string.Format("alter table {0} {1} ", Table.GetQualifiedName(dialect, defaultSchema),
				              SqlConstraintString(dialect, Name, defaultSchema));
		}

		#endregion

		/// <summary>
		/// When implemented by a class, generates the SQL string to create the named
		/// Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Constraint.
		/// </returns>
		public abstract string SqlConstraintString(Dialect.Dialect d, string constraintName, string defaultSchema);
	}
}