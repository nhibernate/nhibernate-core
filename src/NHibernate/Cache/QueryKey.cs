using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache
{
	[Serializable]
	public class QueryKey
	{
		private readonly ISessionFactoryImplementor factory;
		private readonly SqlString sqlQueryString;
		private readonly IType[ ] types;
		private readonly object[ ] values;
		private readonly int firstRow = RowSelection.NoValue;
		private readonly int maxRows = RowSelection.NoValue;
		private readonly IDictionary namedParameters;
		private readonly ISet filters;
		private readonly IResultTransformer customTransformer;
		private readonly int hashCode;

		/// <param name="factory">the sesion factory for this query key, required to get the identifiers of entities that are used as values.</param>
		public QueryKey( ISessionFactoryImplementor factory, SqlString queryString, QueryParameters queryParameters, ISet filters )
		{
			this.factory = factory;
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
			this.filters = filters;
			this.customTransformer = queryParameters.ResultTransformer;
			this.hashCode = ComputeHashCode();
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
			
			if (!Equals(customTransformer, that.customTransformer))
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

			if( !CollectionHelper.DictionaryEquals( namedParameters, that.namedParameters ) )
			{
				return false;
			}
			
			if (!CollectionHelper.SetEquals(filters, that.filters))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public int ComputeHashCode()
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

				if (filters != null)
				{
					foreach (object filter in filters)
					{
						result = 37 * result + filter.GetHashCode();
					}
				}

				result = 37 * result + (customTransformer == null ? 0 : customTransformer.GetHashCode());
				result = 37 * result + sqlQueryString.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			
			StringBuilder buf = new StringBuilder()
				.Append( "sql: " )
				.Append( sqlQueryString );

			Printer print = new Printer(factory);

			if( values != null )
			{
				buf
					.Append("; parameters: ")
					.Append(print.ToString(types, values));
			}
			if( namedParameters != null )
			{
				buf
					.Append("; named parameters: ")
					.Append(print.ToString(namedParameters));
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
