using System;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>for update of ... nowait</c> statement
	/// </summary>
	public class ForUpdateFragment
	{
		private StringBuilder aliases = new StringBuilder();
		private bool nowait;

		/// <summary></summary>
		public ForUpdateFragment()
		{
		}

		/// <summary></summary>
		public bool NoWait
		{
			get { return nowait; }
			set { nowait = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public ForUpdateFragment AddTableAlias( string alias )
		{
			if( aliases.Length > 0 )
			{
				aliases.Append( StringHelper.CommaSpace );
			}
			aliases.Append( alias );
			return this;
		}

		/// <summary></summary>
		public SqlString ToSqlStringFragment( Dialect.Dialect dialect )
		{
			if ( aliases.Length == 0 )
			{
				return new SqlString( String.Empty );
			}
			bool nowait = NoWait && dialect.SupportsForUpdateNoWait;
			if ( dialect.SupportsForUpdateOf )
			{
				return new SqlString( " for update of " + aliases + ( nowait ? " nowait" : String.Empty ) );
			}
			else if ( dialect.SupportsForUpdate )
			{
				return new SqlString( " for update" + ( nowait ? " nowait" : String.Empty ) );
			}
			else
				return new SqlString( String.Empty );
		}

	}
}