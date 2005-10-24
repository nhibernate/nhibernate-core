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
		public string SqlConstraintString( Dialect.Dialect d )
		{
			StringBuilder buf = new StringBuilder( " unique (" );
			bool commaNeeded = false;

			foreach( Column col in ColumnCollection )
			{
				if( commaNeeded )
				{
					buf.Append( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				buf.Append( col.GetQuotedName( d ) );

			}

			return buf.Append( StringHelper.ClosedParen ).ToString();
		}

		/// <summary>
		/// Generates the SQL string to create the Unique Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the Unique Key Constraint.
		/// </returns>
		public override string SqlConstraintString( Dialect.Dialect d, string constraintName, string defaultSchema )
		{
			StringBuilder buf = new StringBuilder(
				d.GetAddPrimaryKeyConstraintString( constraintName ) )
				.Append( '(' );

			bool commaNeeded = false;

			foreach( Column col in ColumnCollection )
			{
				if( commaNeeded )
				{
					buf.Append( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				buf.Append( col.GetQuotedName( d ) );

			}

			return StringHelper.Replace( buf.Append( StringHelper.ClosedParen ).ToString(), "primary key", "unique" );
		}

		#region IRelationalModel Members

		/// <summary>
 		/// Get the SQL string to drop this Constraint in the database.
 		/// </summary>
 		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
 		/// A string that contains the SQL to drop this Constraint.
 		/// </returns>
 		public override string SqlDropString(NHibernate.Dialect.Dialect dialect, string defaultSchema )
		{
			// TODO: NH-421
 			return "alter table " + Table.GetQualifiedName( dialect, defaultSchema ) + dialect.GetDropIndexConstraintString( Name );
 		}

		#endregion
	}
}