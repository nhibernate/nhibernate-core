using System.Collections;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an <c>... in (...)</c> expression
	/// </summary>
	public class InFragment
	{
		private string columnName;
		private ArrayList values = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public InFragment AddValue( string value )
		{
			values.Add( value );
			return this;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public InFragment SetColumn( string columnName )
		{
			this.columnName = columnName;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public InFragment SetColumn( string alias, string columnName )
		{
			this.columnName = alias + StringHelper.Dot + columnName;
			return SetColumn( this.columnName );
		}

		/// <summary></summary>
		public SqlString ToFragmentString()
		{
			SqlStringBuilder buf = new SqlStringBuilder( values.Count*5 );
			buf.Add( columnName );

			if( values.Count > 1 )
			{
				// is a comma needed before the value that's about to be added - it
				// defaults to false because we don't need a comma right away.
				bool commaNeeded = false;

				// if a "null" is in the list of values then we need to manipulate
				// the SqlString a little bit more at the end.
				bool allowNull = false;

				buf.Add( " in (" );
				for( int i = 0; i < values.Count; i++ )
				{
					if( "null".Equals( values[ i ] ) )
					{
						allowNull = true;
					}
					else
					{
						if( commaNeeded )
						{
							buf.Add( StringHelper.CommaSpace );
						}
						buf.Add( ( string ) values[ i ] );

						// a value has been added into the IN clause so the next
						// one needs a comma before it
						commaNeeded = true;
					}
				}

				buf.Add( StringHelper.ClosedParen );

				// if "null" is in the list of values then add to the beginning of the
				// SqlString "is null or [column] (" + [rest of sqlstring here] + ")"
				if( allowNull )
				{
					buf.Insert( 0, " is null or " )
						.Insert( 0, columnName )
						.Insert( 0, StringHelper.OpenParen )
						.Add( StringHelper.ClosedParen );
				}
			}
			else
			{
				string value = values[ 0 ] as string;
				if( "null".Equals( value ) )
				{
					buf.Add( " is null" );
				}
				else
				{
					buf.Add( "=" + values[ 0 ] );
				}
			}
			return buf.ToSqlString();
		}
	}
}