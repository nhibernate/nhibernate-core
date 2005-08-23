using System;
using System.Collections;
using System.Text;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache
{
	[Serializable]
	public class QueryKey
	{
		private readonly SqlString sqlQueryString;
		private readonly IType[ ] types;
		private readonly object[ ] values;
		private readonly int firstRow = RowSelection.NoValue;
		private readonly int maxRows = RowSelection.NoValue;
		private readonly IDictionary namedParameters;

		public QueryKey( SqlString queryString, QueryParameters queryParameters )
		{
			sqlQueryString = queryString;
			types = queryParameters.PositionalParameterTypes;
			values = queryParameters.PositionalParameterValues;

			RowSelection selection = queryParameters.RowSelection;
			if( selection != null )
			{
				firstRow = selection.FirstRow;
				maxRows = selection.MaxRows;
			}
			else
			{
				firstRow = RowSelection.NoValue;
				maxRows = RowSelection.NoValue;
			}
			namedParameters = queryParameters.NamedParameters;
		}

		private static bool DictionariesAreEqual( IDictionary a, IDictionary b )
		{
			if( a == null && b == null )
			{
				return true;
			}

			if( a == null && b != null )
			{
				return false;
			}

			if( a != null && b == null )
			{
				return false;
			}

			if( a.Count != b.Count )
			{
				return false;
			}

			foreach( object key in a.Keys )
			{
				if( !object.Equals( a[ key ], b[ key ] ) )
				{
					return false;
				}
			}

			return true;
		}

		public override bool Equals( object other )
		{
			QueryKey that = ( QueryKey ) other;
			if( !sqlQueryString.Equals( that.sqlQueryString ) )
			{
				return false;
			}
			if( firstRow != that.firstRow
				|| maxRows != that.maxRows )
			{
				return false;
			}

			if( types == null )
			{
				if( that.types != null )
				{
					return false;
				}
			}
			else
			{
				if( that.types == null )
				{
					return false;
				}
				if( types.Length != that.types.Length )
				{
					return false;
				}

				for( int i = 0; i < types.Length; i++ )
				{
					if( !types[ i ].Equals( that.types[ i ] ) )
					{
						return false;
					}
					if( !object.Equals( values[ i ], that.values[ i ] ) )
					{
						return false;
					}
				}
			}

			if( !DictionariesAreEqual( namedParameters, that.namedParameters ) )
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = 13;
				result = 37 * result + firstRow.GetHashCode();
				result = 37 * result + maxRows.GetHashCode();

				// NH - commented this out, namedParameters don't have a useful GetHashCode implementations
				//result = 37 * result
				//	+ ( namedParameters == null ? 0 : namedParameters.GetHashCode() );

				for( int i = 0; i < types.Length; i++ )
				{
					result = 37 * result + ( types[ i ] == null ? 0 : types[ i ].GetHashCode() );
				}
				for( int i = 0; i < values.Length; i++ )
				{
					result = 37 * result + ( values[ i ] == null ? 0 : values[ i ].GetHashCode() );
				}
				result = 37 * result + sqlQueryString.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder()
				.Append( "sql: " )
				.Append( sqlQueryString );

			if( values != null )
			{
				buf.Append( "; parameters: " );
				for( int i = 0; i < values.Length; i++ )
				{
					buf.Append( values[ i ] )
						.Append( ", " );
				}
			}
			if( namedParameters != null )
			{
				buf.Append( "; named parameters: " )
					.Append( CollectionPrinter.ToString( namedParameters ) );
			}
			if( firstRow != RowSelection.NoValue )
			{
				buf.Append( "; first row: " ).Append( firstRow );
			}
			if( maxRows != RowSelection.NoValue )
			{
				buf.Append( "; max rows: " ).Append( maxRows );
			}

			return buf.ToString();
		}
	}
}