using System.Text;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Primary Key constraint in the database.
	/// </summary>
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
		public string SqlConstraintString( Dialect.Dialect d, string defaultSchema )
		{
			StringBuilder buf = new StringBuilder( " primary key (" );
			int i = 0;
			foreach( Column col in ColumnCollection )
			{
				buf.Append( col.GetQuotedName( d ) );
				if( i < ColumnCollection.Count - 1 )
				{
					buf.Append( StringHelper.CommaSpace );
				}
				i++;
			}
			return buf.Append( StringHelper.ClosedParen ).ToString();
		}

		/// <summary>
		/// Generates the SQL string to create the named Primary Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Primary Key Constraint.
		/// </returns>
		public override string SqlConstraintString( Dialect.Dialect d, string constraintName, string defaultSchema )
		{
			StringBuilder buf = new StringBuilder(
				d.GetAddPrimaryKeyConstraintString( constraintName ) )
				.Append( '(' );
			int i = 0;
			foreach( Column col in ColumnCollection )
			{
				buf.Append( col.GetQuotedName( d ) );
				if( i < ColumnCollection.Count - 1 )
				{
					buf.Append( StringHelper.CommaSpace );
				}
				i++;
			}
			return buf.Append( StringHelper.ClosedParen ).ToString();
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
			return "alter table " + Table.GetQualifiedName( dialect, defaultSchema ) + dialect.GetDropPrimaryKeyConstraintString( Name );
 		}
		#endregion
	}
}