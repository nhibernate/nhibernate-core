using System.Text;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class PrimaryKey : Constraint
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public string SqlConstraintString( Dialect.Dialect d )
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
	}
}