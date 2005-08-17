using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// A key that identifies a particular query with bound parameter values
	/// </summary>
	public class QueryKey
	{
		private readonly SqlString sqlQueryString;
		private readonly IType[] types;
		private readonly object[] values;
		private readonly int firstRow;
		private readonly int maxRows;
		private readonly IDictionary namedParameters;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="queryParameters"></param>
		public QueryKey( SqlString queryString, QueryParameters queryParameters )
		{
			this.sqlQueryString = queryString;
			this.types = queryParameters.PositionalParameterTypes;
			this.values = queryParameters.PositionalParameterValues;
			RowSelection selection = queryParameters.RowSelection;
			if ( selection != null )
			{
				firstRow = selection.FirstRow;
				maxRows = selection.MaxRows;
			}
			else
			{
				firstRow = -1;
				maxRows = -1;
			}
			this.namedParameters = queryParameters.NamedParameters;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals( object other )
		{
			QueryKey that = other as QueryKey;
			if ( sqlQueryString != that.sqlQueryString )
			{
				return false;
			}
			if ( firstRow != that.firstRow || maxRows != that.maxRows )
			{
				return false;
			}

			if ( types == null )
			{
				if ( that.types != null )
				{
					return false;
				}
			}
			else
			{
				if ( that.types == null )
				{
					return false;
				}
				if ( types.Length != that.types.Length )
				{
					return false;
				}
				for ( int i = 0; i < types.Length; i++ )
				{
					if ( !types[ i ].Equals( that.types[ i ] ) )
					{
						return false;
					}
					if ( !values[ i ].Equals( that.values[ i ] ) )
					{
						return false;
					}
				}
			}
			// TODO: Probably need to ensure we check the members rather than just reference equality!
			if ( !namedParameters.Equals( that.namedParameters ) )
			{
				return false; 
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int result = 13;
			result = 37 * result + ( firstRow == -1 ? 0 : firstRow.GetHashCode() );
			result = 37 * result + ( maxRows == -1 ? 0 : maxRows.GetHashCode() );
			result = 37 * result + ( namedParameters == null ? 0 : namedParameters.GetHashCode() );
			for ( int i = 0; i < types.Length; i++ ) 
			{
				result = 37 * result + ( types[i] == null ? 0 : types[i].GetHashCode() );
			}
			for ( int i = 0; i < values.Length; i++ ) 
			{
				result = 37 * result + ( values[i] == null ? 0 : values[i].GetHashCode() );
			}
			result = 37 * result + sqlQueryString.GetHashCode();
			return result;
		}
	}
}