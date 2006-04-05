using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>for update of ... nowait</c> statement
	/// </summary>
	public class ForUpdateFragment
	{
		private Dialect.Dialect dialect;
		private StringBuilder aliases = new StringBuilder();
		private bool nowait;

		public ForUpdateFragment( Dialect.Dialect dialect )
		{
			this.dialect = dialect;
		}

		public ForUpdateFragment( Dialect.Dialect dialect, IDictionary lockModes, IDictionary keyColumnNames )
			: this( dialect )
		{
			LockMode upgradeType = null;
			
			foreach( DictionaryEntry me in lockModes )
			{
				LockMode lockMode = ( LockMode ) me.Value;
				if ( LockMode.Read.LessThan(lockMode) )
				{
					string tableAlias = ( string ) me.Key;
					if( dialect.ForUpdateOfColumns )
					{
						string[] keyColumns = ( string[] ) keyColumnNames[ tableAlias ];
						if( keyColumns == null )
						{
							throw new ArgumentException( "alias not found: " + tableAlias );
						}
						keyColumns = StringHelper.Qualify( tableAlias, keyColumns );
						for( int i = 0; i < keyColumns.Length; i++ )
						{
							AddTableAlias( keyColumns[ i ] );
						}
					}
					else
					{
						AddTableAlias( tableAlias );
					}
					if ( upgradeType != null && lockMode != upgradeType )
					{
						throw new QueryException("mixed LockModes");
					}
					upgradeType = lockMode;
				}
				
				if ( upgradeType == LockMode.UpgradeNoWait ){
					NoWait = true;
				}
			}
		}

		public bool NoWait
		{
			get { return nowait; }
			set { nowait = value; }
		}

		public ForUpdateFragment AddTableAlias( string alias )
		{
			if( aliases.Length > 0 )
			{
				aliases.Append( StringHelper.CommaSpace );
			}
			aliases.Append( alias );
			return this;
		}

		public SqlString ToSqlStringFragment()
		{
			if ( aliases.Length == 0 )
			{
				return SqlString.Empty;
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