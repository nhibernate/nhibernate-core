using System;
using System.Collections.Generic;
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
		private readonly IType[] types;
		private readonly object[] values;
		private readonly int firstRow = RowSelection.NoValue;
		private readonly int maxRows = RowSelection.NoValue;
		private readonly IDictionary<string, TypedValue> namedParameters;
		private readonly ISet filters;
		private readonly IResultTransformer customTransformer;
		private readonly int hashCode;

		private int[] multiQueriesFirstRows;
		private int[] multiQueriesMaxRows;


		/// <summary>
		/// Initializes a new instance of the <see cref="QueryKey"/> class.
		/// </summary>
		/// <param name="factory">the session factory for this query key, required to get the identifiers of entities that are used as values.</param>
		/// <param name="queryString">The query string.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="filters">The filters.</param>
		public QueryKey(ISessionFactoryImplementor factory, SqlString queryString, QueryParameters queryParameters,
		                ISet filters)
		{
			this.factory = factory;
			sqlQueryString = queryString;
			types = queryParameters.PositionalParameterTypes;
			values = queryParameters.PositionalParameterValues;

			RowSelection selection = queryParameters.RowSelection;
			if (selection != null)
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
			customTransformer = queryParameters.ResultTransformer;
			hashCode = ComputeHashCode();
		}

		public QueryKey SetFirstRows(int[] firstRows)
		{
			multiQueriesFirstRows = firstRows;
			return this;
		}

		public QueryKey SetMaxRows(int[] maxRows)
		{
			multiQueriesMaxRows = maxRows;
			return this;
		}

		public override bool Equals(object other)
		{
			QueryKey that = (QueryKey) other;
			if (!sqlQueryString.Equals(that.sqlQueryString))
			{
				return false;
			}
			if (firstRow != that.firstRow
			    || maxRows != that.maxRows)
			{
				return false;
			}

			if (!Equals(customTransformer, that.customTransformer))
			{
				return false;
			}

			if (types == null)
			{
				if (that.types != null)
				{
					return false;
				}
			}
			else
			{
				if (that.types == null)
				{
					return false;
				}
				if (types.Length != that.types.Length)
				{
					return false;
				}

				for (int i = 0; i < types.Length; i++)
				{
					if (!types[i].Equals(that.types[i]))
					{
						return false;
					}
					if (!Equals(values[i], that.values[i]))
					{
						return false;
					}
				}
			}

			if (!CollectionHelper.SetEquals(filters, that.filters))
			{
				return false;
			}

			if (!CollectionHelper.DictionaryEquals(namedParameters, that.namedParameters))
			{
				return false;
			}

			if(!CollectionHelper.CollectionEquals<int>(multiQueriesFirstRows, that.multiQueriesFirstRows))
			{
				return false;
			}
			if(!CollectionHelper.CollectionEquals<int>(multiQueriesMaxRows, that.multiQueriesMaxRows))
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

				result = 37 * result + ( namedParameters == null ? 0: CollectionHelper.GetHashCode(namedParameters));

				for (int i = 0; i < types.Length; i++)
				{
					result = 37 * result + (types[i] == null ? 0 : types[i].GetHashCode());
				}
				for (int i = 0; i < values.Length; i++)
				{
					result = 37 * result + (values[i] == null ? 0 : values[i].GetHashCode());
				}

				if(multiQueriesFirstRows!=null)
				{
					foreach (int multiQueriesFirstRow in multiQueriesFirstRows)
					{
						result = 37*result + multiQueriesFirstRow;
					}
				}

				if(multiQueriesMaxRows!=null)
				{
					foreach (int multiQueriesMaxRow in multiQueriesMaxRows)
					{
						result = 37*result + multiQueriesMaxRow;
					}
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
				.Append("sql: ")
				.Append(sqlQueryString);

			Printer print = new Printer(factory);

			if (values != null)
			{
				buf
					.Append("; parameters: ")
					.Append(print.ToString(types, values));
			}
			if (namedParameters != null)
			{
				buf
					.Append("; named parameters: ")
					.Append(print.ToString(namedParameters));
			}
			if (filters != null)
			{
				buf.Append("; filters: ").Append(CollectionPrinter.ToString(filters));
			}
			if (firstRow != RowSelection.NoValue)
			{
				buf.Append("; first row: ").Append(firstRow);
			}
			if (maxRows != RowSelection.NoValue)
			{
				buf.Append("; max rows: ").Append(maxRows);
			}

			if(multiQueriesFirstRows!=null)
			{
				buf.Append("; multi queries - first rows: ");
				for (int i = 0; i < multiQueriesFirstRows.Length; i++)
				{
					buf.Append("#").Append(i)
						.Append("=")
						.Append(multiQueriesFirstRows[i]);
				}
				buf.Append("; ");
			}

			if(multiQueriesMaxRows!=null)
			{
				buf.Append("; multi queries - max rows: ");
				for (int i = 0; i < multiQueriesMaxRows.Length; i++)
				{
					buf.Append("#").Append(i)
						.Append("=")
						.Append(multiQueriesMaxRows[i]);
				}
				buf.Append("; ");
			}


			return buf.ToString();
		}
	}
}
