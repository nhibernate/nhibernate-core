using System.Text;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class UniqueKey : Constraint
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="constraintName"></param>
		/// <returns></returns>
		public override string SqlConstraintString( Dialect.Dialect d, string constraintName )
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
	}
}