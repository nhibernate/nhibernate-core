using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
	public class QueryKey : IDeserializationCallback, IEquatable<QueryKey>
	{
		private readonly ISessionFactoryImplementor _factory;
		private readonly SqlString _sqlQueryString;
		private readonly IType[] _types;
		private readonly object[] _values;
		private readonly int _firstRow = RowSelection.NoValue;
		private readonly int _maxRows = RowSelection.NoValue;

		// Sets and dictionaries are populated last during deserialization, causing them to be potentially empty
		// during the deserialization callback. This causes them to be unreliable when used in hashcode or equals
		// computations. These computations occur during the deserialization callback for example when another
		// serialized set or dictionary contain an instance of this class.
		// So better serialize them as other structures, so long for Equals implementation which actually needs a
		// dictionary and set.
		private readonly KeyValuePair<string, TypedValue>[] _namedParameters;
		private readonly FilterKey[] _filters;

		private readonly CacheableResultTransformer _customTransformer;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		[NonSerialized]
		private int? _hashCode;

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

			_namedParameters = queryParameters.NamedParameters?.ToArray();
			_filters = filters?.ToArray();
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
			return Equals(other as QueryKey);
		}

		public bool Equals(QueryKey other)
		{
			if (other == null || !_sqlQueryString.Equals(other._sqlQueryString))
			{
				return false;
			}

			if (_firstRow != other._firstRow || _maxRows != other._maxRows)
			{
				return false;
			}

			if (!Equals(_customTransformer, other._customTransformer))
			{
				return false;
			}

			if (_types == null)
			{
				if (other._types != null)
				{
					return false;
				}
			}
			else
			{
				if (other._types == null)
				{
					return false;
				}
				if (_types.Length != other._types.Length)
				{
					return false;
				}

				for (int i = 0; i < _types.Length; i++)
				{
					if (!_types[i].Equals(other._types[i]))
					{
						return false;
					}
					if (!Equals(_values[i], other._values[i]))
					{
						return false;
					}
				}
			}

			// BagEquals is less efficient than a SetEquals or DictionaryEquals, but serializing dictionaries causes
			// issues on deserialization if GetHashCode or Equals are called in its deserialization callback. And
			// building sets or dictionaries on the fly will in most cases be worst than BagEquals, unless re-coding
			// its short-circuits.
			if (!CollectionHelper.BagEquals(_filters, other._filters))
				return false;
			if (!CollectionHelper.BagEquals(_namedParameters, other._namedParameters, NamedParameterComparer.Instance))
				return false;

			if (!CollectionHelper.SequenceEquals(_multiQueriesFirstRows, other._multiQueriesFirstRows))
			{
				return false;
			}
			if (!CollectionHelper.SequenceEquals(_multiQueriesMaxRows, other._multiQueriesMaxRows))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			// If the object is put in a set or dictionary during deserialization, the hashcode will not yet be
			// computed. Compute the hashcode on the fly. So long as this happens only during deserialization, there
			// will be no thread safety issues. For the hashcode to be always defined after deserialization, the
			// deserialization callback is used.
			return _hashCode ?? ComputeHashCode();
		}

		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			_hashCode = ComputeHashCode();
		}

		public int ComputeHashCode()
		{
			unchecked
			{
				int result = 13;
				result = 37 * result + _firstRow.GetHashCode();
				result = 37 * result + _maxRows.GetHashCode();

				result = 37 * result + (_namedParameters == null
					? 0
					: CollectionHelper.GetHashCode(_namedParameters, NamedParameterComparer.Instance));

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
					result = 37 * result + CollectionHelper.GetHashCode(_filters);
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
