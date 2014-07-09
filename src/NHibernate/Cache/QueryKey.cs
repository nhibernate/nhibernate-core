using System;
using System.Collections.Generic;
using System.Text;
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
		private readonly ISessionFactoryImplementor _factory;
		private readonly SqlString _sqlQueryString;
		private readonly IType[] _types;
		private readonly object[] _values;
		private readonly int _firstRow = RowSelection.NoValue;
		private readonly int _maxRows = RowSelection.NoValue;
		private readonly IDictionary<string, TypedValue> _namedParameters;
		private readonly ISet<FilterKey> _filters;
		private readonly CacheableResultTransformer _customTransformer;
		private readonly int _hashCode;

		private int[] _multiQueriesFirstRows;
		private int[] _multiQueriesMaxRows;


		/// <summary>
		/// Initializes a new instance of the <see cref="QueryKey"/> class.
		/// </summary>
		/// <param name="factory">the session factory for this query key, required to get the identifiers of entities that are used as values.</param>
		/// <param name="queryString">The query string.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="filters">The filters.</param>
		/// <param name="customTransformer">The result transformer; should be null if data is not transformed before being cached.</param>
		public QueryKey(ISessionFactoryImplementor factory, SqlString queryString, QueryParameters queryParameters,
		                ISet<FilterKey> filters, CacheableResultTransformer customTransformer)
		{
			_factory = factory;
			_sqlQueryString = queryString;
			_types = queryParameters.PositionalParameterTypes;
			_values = queryParameters.PositionalParameterValues;

			RowSelection selection = queryParameters.RowSelection;
			if (selection != null)
			{
				_firstRow = selection.FirstRow;
				_maxRows = selection.MaxRows;
			}
			else
			{
				_firstRow = RowSelection.NoValue;
				_maxRows = RowSelection.NoValue;
			}
			_namedParameters = queryParameters.NamedParameters;
			_filters = filters;
			_customTransformer = customTransformer;
			_hashCode = ComputeHashCode();
		}

		public CacheableResultTransformer ResultTransformer
		{
			get { return _customTransformer; }
		}

		public QueryKey SetFirstRows(int[] firstRows)
		{
			_multiQueriesFirstRows = firstRows;
			return this;
		}

		public QueryKey SetMaxRows(int[] maxRows)
		{
			_multiQueriesMaxRows = maxRows;
			return this;
		}

		public override bool Equals(object other)
		{
			QueryKey that = (QueryKey)other;
			if (!_sqlQueryString.Equals(that._sqlQueryString))
			{
				return false;
			}
			if (_firstRow != that._firstRow
				|| _maxRows != that._maxRows)
			{
				return false;
			}

			if (!Equals(_customTransformer, that._customTransformer))
			{
				return false;
			}

			if (_types == null)
			{
				if (that._types != null)
				{
					return false;
				}
			}
			else
			{
				if (that._types == null)
				{
					return false;
				}
				if (_types.Length != that._types.Length)
				{
					return false;
				}

				for (int i = 0; i < _types.Length; i++)
				{
					if (!_types[i].Equals(that._types[i]))
					{
						return false;
					}
					if (!Equals(_values[i], that._values[i]))
					{
						return false;
					}
				}
			}

			if (!CollectionHelper.SetEquals(_filters, that._filters))
			{
				return false;
			}

			if (!CollectionHelper.DictionaryEquals(_namedParameters, that._namedParameters))
			{
				return false;
			}

			if (!CollectionHelper.CollectionEquals<int>(_multiQueriesFirstRows, that._multiQueriesFirstRows))
			{
				return false;
			}
			if (!CollectionHelper.CollectionEquals<int>(_multiQueriesMaxRows, that._multiQueriesMaxRows))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}

		public int ComputeHashCode()
		{
			unchecked
			{
				int result = 13;
				result = 37 * result + _firstRow.GetHashCode();
				result = 37 * result + _maxRows.GetHashCode();

				result = 37 * result + (_namedParameters == null ? 0 : CollectionHelper.GetHashCode(_namedParameters));

				for (int i = 0; i < _types.Length; i++)
				{
					result = 37 * result + (_types[i] == null ? 0 : _types[i].GetHashCode());
				}
				for (int i = 0; i < _values.Length; i++)
				{
					result = 37 * result + (_values[i] == null ? 0 : _values[i].GetHashCode());
				}

				if (_multiQueriesFirstRows != null)
				{
					foreach (int multiQueriesFirstRow in _multiQueriesFirstRows)
					{
						result = 37 * result + multiQueriesFirstRow;
					}
				}

				if (_multiQueriesMaxRows != null)
				{
					foreach (int multiQueriesMaxRow in _multiQueriesMaxRows)
					{
						result = 37 * result + multiQueriesMaxRow;
					}
				}

				if (_filters != null)
				{
					foreach (object filter in _filters)
					{
						result = 37 * result + filter.GetHashCode();
					}
				}

				result = 37 * result + (_customTransformer == null ? 0 : _customTransformer.GetHashCode());
				result = 37 * result + _sqlQueryString.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder()
				.Append("sql: ")
				.Append(_sqlQueryString);

			Printer print = new Printer(_factory);

			if (_values != null)
			{
				buf
					.Append("; parameters: ")
					.Append(print.ToString(_types, _values));
			}
			if (_namedParameters != null)
			{
				buf
					.Append("; named parameters: ")
					.Append(print.ToString(_namedParameters));
			}
			if (_filters != null)
			{
				buf.Append("; filters: ").Append(CollectionPrinter.ToString(_filters));
			}
			if (_firstRow != RowSelection.NoValue)
			{
				buf.Append("; first row: ").Append(_firstRow);
			}
			if (_maxRows != RowSelection.NoValue)
			{
				buf.Append("; max rows: ").Append(_maxRows);
			}

			if (_multiQueriesFirstRows != null)
			{
				buf.Append("; multi queries - first rows: ");
				for (int i = 0; i < _multiQueriesFirstRows.Length; i++)
				{
					buf.Append("#").Append(i)
						.Append("=")
						.Append(_multiQueriesFirstRows[i]);
				}
				buf.Append("; ");
			}

			if (_multiQueriesMaxRows != null)
			{
				buf.Append("; multi queries - max rows: ");
				for (int i = 0; i < _multiQueriesMaxRows.Length; i++)
				{
					buf.Append("#").Append(i)
						.Append("=")
						.Append(_multiQueriesMaxRows[i]);
				}
				buf.Append("; ");
			}


			return buf.ToString();
		}
	}
}
