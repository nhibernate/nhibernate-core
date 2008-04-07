using System;
using System.Collections;
using log4net;
using NHibernate.Hql.Classic;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary>
	/// Container for data that is used during the NHibernate query/load process. 
	/// </summary>
	[Serializable]
	public sealed class QueryParameters
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(QueryParameters));

		private IType[] _positionalParameterTypes;
		private object[] _positionalParameterValues;
		private IDictionary _namedParameters;
		private IDictionary _lockModes;
		private RowSelection _rowSelection;
		private bool _cacheable;
		private string _cacheRegion;
		private bool _forceCacheRefresh;
		private object[] _collectionKeys;
		private object _optionalObject;
		private string _optionalEntityName;
		private object _optionalId;
		private string _comment;
		private bool _isNaturalKeyLookup;
		private bool _readOnly;
		private bool _callable;
		private bool autoDiscoverTypes;

		private SqlString processedSQL;
		private IType[] processedPositionalParameterTypes;
		private object[] processedPositionalParameterValues;

		private IResultTransformer _resultTransformer;
		// not implemented: private ScrollMode _scrollMode;

		public QueryParameters()
			: this(ArrayHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray)
		{
		}

		public QueryParameters(IType type, object value)
			: this(new IType[] { type }, new object[] { value })
		{
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues,
			object optionalObject, string optionalEntityName, object optionalObjectId)
			: this(positionalParameterTypes, postionalParameterValues)
		{
			_optionalObject = optionalObject;
			_optionalId = optionalObjectId;
			_optionalEntityName = optionalEntityName;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues)
			: this(positionalParameterTypes, postionalParameterValues, null, null, false, null, null, false, null)
		{
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues,
			object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, null, collectionKeys)
		{
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues,
			IDictionary namedParameters, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, namedParameters, null, null, false, false, null, null, collectionKeys, null)
		{
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues,
			IDictionary lockModes, RowSelection rowSelection, bool cacheable, string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer)
			: this(positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection, false, cacheable, cacheRegion, comment, null, transformer)
		{
			_isNaturalKeyLookup = isLookupByNaturalKey;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues,
			IDictionary namedParameters, IDictionary lockModes, RowSelection rowSelection,
			bool readOnly, bool cacheable, string cacheRegion, string comment,
			object[] collectionKeys, IResultTransformer transformer)
		{
			_positionalParameterTypes = positionalParameterTypes;
			_positionalParameterValues = positionalParameterValues;
			_namedParameters = namedParameters;
			_lockModes = lockModes;
			_rowSelection = rowSelection;
			_cacheable = cacheable;
			_cacheRegion = cacheRegion;
			_comment = comment;
			_collectionKeys = collectionKeys;
			_readOnly = readOnly;
			_resultTransformer = transformer;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues,
			IDictionary namedParameters, IDictionary lockModes, RowSelection rowSelection,
			bool readOnly, bool cacheable, string cacheRegion, string comment, object[] collectionKeys,
			object optionalObject, string optionalEntityName, object optionalId, IResultTransformer transformer)
			: this(positionalParameterTypes, positionalParameterValues, namedParameters, lockModes, rowSelection, readOnly, cacheable, cacheRegion, comment, collectionKeys, transformer)
		{
			_optionalEntityName = optionalEntityName;
			_optionalId = optionalId;
			_optionalObject = optionalObject;
		}

		/// <summary></summary>
		public bool HasRowSelection
		{
			get { return _rowSelection != null; }
		}

		/// <summary>
		/// Gets or sets an <see cref="IDictionary"/> that contains the named 
		/// parameter as the key and the <see cref="TypedValue"/> as the value.
		/// </summary>
		/// <value>An <see cref="IDictionary"/> of named parameters.</value>
		public IDictionary NamedParameters
		{
			get { return _namedParameters; }
			set { _namedParameters = value; }
		}

		/// <summary>
		/// Gets or sets an array of <see cref="IType"/> objects that is stored at the index 
		/// of the Parameter.
		/// </summary>
		public IType[] PositionalParameterTypes
		{
			get { return _positionalParameterTypes; }
			set { _positionalParameterTypes = value; }
		}

		/// <summary>
		/// Gets or sets an array of <see cref="object"/> objects that is stored at the index 
		/// of the Parameter.
		/// </summary>
		public object[] PositionalParameterValues
		{
			get { return _positionalParameterValues; }
			set { _positionalParameterValues = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="RowSelection"/> for the Query.
		/// </summary>
		public RowSelection RowSelection
		{
			get { return _rowSelection; }
			set { _rowSelection = value; }
		}

		/// <summary>
		/// Gets or sets an <see cref="IDictionary"/> that contains the alias name of the
		/// object from hql as the key and the <see cref="LockMode"/> as the value.
		/// </summary>
		/// <value>An <see cref="IDictionary"/> of lock modes.</value>
		public IDictionary LockModes
		{
			get { return _lockModes; }
			set { _lockModes = value; }
		}

		private int SafeLength(Array array)
		{
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}

		/// <summary></summary>
		public void LogParameters(ISessionFactoryImplementor factory)
		{
			Printer print = new Printer(factory);
			if (_positionalParameterValues.Length != 0)
			{
				log.Debug("parameters: "
									+ print.ToString(_positionalParameterTypes, _positionalParameterValues));
			}

			if (_namedParameters != null)
			{
				log.Debug("named parameters: "
									+ print.ToString(_namedParameters));
			}
		}

		public bool Cacheable
		{
			get { return _cacheable; }
			set { _cacheable = value; }
		}

		public string CacheRegion
		{
			get { return _cacheRegion; }
			set { _cacheRegion = value; }
		}

		/// <summary>
		/// Ensure the Types and Values are the same length.
		/// </summary>
		/// <exception cref="QueryException">
		/// If the Lengths of <see cref="PositionalParameterTypes"/> and 
		/// <see cref="PositionalParameterValues"/> are not equal.
		/// </exception>
		public void ValidateParameters()
		{
			int typesLength = SafeLength(PositionalParameterTypes);
			int valuesLength = SafeLength(PositionalParameterValues);

			if (typesLength != valuesLength)
			{
				throw new QueryException("Number of positional parameter types (" + typesLength +
																 ") does not match number of positional parameter values (" + valuesLength + ")");
			}
		}

		public bool ForceCacheRefresh
		{
			get { return _forceCacheRefresh; }
			set { _forceCacheRefresh = value; }
		}

		public string OptionalEntityName
		{
			get { return _optionalEntityName; }
			set { _optionalEntityName = value; }
		}

		public object OptionalId
		{
			get { return _optionalId; }
			set { _optionalId = value; }
		}

		public object OptionalObject
		{
			get { return _optionalObject; }
			set { _optionalObject = value; }
		}

		public object[] CollectionKeys
		{
			get { return _collectionKeys; }
			set { _collectionKeys = value; }
		}

		public bool Callable
		{
			get { return _callable; }
			set { _callable = value; }
		}

		public bool ReadOnly
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}

		/************** Filters ********************************/

		public void ProcessFilters(SqlString sql, ISessionImplementor session)
		{
			if (session.EnabledFilters.Count == 0 || sql.ToString().IndexOf(ParserHelper.HqlVariablePrefix) < 0)
			{
				processedPositionalParameterValues = PositionalParameterValues;
				processedPositionalParameterTypes = PositionalParameterTypes;
				processedSQL = sql;
			}
			else
			{
				Dialect.Dialect dialect = session.Factory.Dialect;
				string symbols = ParserHelper.HqlSeparators + dialect.OpenQuote + dialect.CloseQuote;

				SqlStringBuilder result = new SqlStringBuilder();

				IList parameters = new ArrayList();
				IList parameterTypes = new ArrayList();
				int parameterCount = 0; // keep track of the positional parameter

				foreach (object part in sql.Parts)
				{
					if (part is Parameter)
					{
						result.AddParameter();

						// (?) can be a position parameter or a named parameter (already substituted by (?),
						// but only the positional parameters are available at this point. Adding them in the
						// order of appearance is best that can be done at this point of time, but if they
						// are mixed with named parameters, the order is still wrong, because values and
						// types for the named parameters are added later to the end of the list.
						// see test fixture NH-1098
						if (parameterCount < PositionalParameterValues.Length)
						{
							parameters.Add(PositionalParameterValues[parameterCount]);
							parameterTypes.Add(PositionalParameterTypes[parameterCount]);
							parameterCount++;
						}

						continue;
					}

					StringTokenizer tokenizer = new StringTokenizer((string)part, symbols, true);

					foreach (string token in tokenizer)
					{
						if (token.StartsWith(ParserHelper.HqlVariablePrefix))
						{
							string filterParameterName = token.Substring(1);
							object value = session.GetFilterParameterValue(filterParameterName);
							IType type = session.GetFilterParameterType(filterParameterName);

							// If the value is not a value of the type but a collection of values...
							if (value != null &&
									!type.ReturnedClass.IsAssignableFrom(value.GetType()) && // Added to fix NH-882
									typeof(ICollection).IsAssignableFrom(value.GetType()))
							{
								ICollection coll = (ICollection)value;
								int i = 0;
								foreach (object elementValue in coll)
								{
									i++;
									int span = type.GetColumnSpan(session.Factory);
									if (span > 0)
									{
										result.AddParameter();
										parameters.Add(elementValue);
										parameterTypes.Add(type);
										if (i < coll.Count)
											result.Add(", ");
									}
								}
							}
							else
							{
								int span = type.GetColumnSpan(session.Factory);
								if (span > 0)
								{
									result.AddParameter();
									parameters.Add(value);
									parameterTypes.Add(type);
								}
							}
						}
						else
						{
							result.Add(token);
						}
					}
				}

				processedPositionalParameterValues = ((ArrayList)parameters).ToArray();
				processedPositionalParameterTypes = (IType[])((ArrayList)parameterTypes).ToArray(typeof(IType));
				processedSQL = result.ToSqlString();
			}
		}

		public SqlString FilteredSQL
		{
			get { return processedSQL; }
		}

		public object[] FilteredPositionalParameterValues
		{
			get { return processedPositionalParameterValues; }
		}

		public IType[] FilteredPositionalParameterTypes
		{
			get { return processedPositionalParameterTypes; }
		}

		public IResultTransformer ResultTransformer
		{
			get { return _resultTransformer; }
		}

		public bool HasAutoDiscoverScalarTypes
		{
			get { return autoDiscoverTypes; }
			set { autoDiscoverTypes = value; }
		}

		public QueryParameters CreateCopyUsing(RowSelection selection)
		{
			QueryParameters copy = new QueryParameters(_positionalParameterTypes, _positionalParameterValues,
				_namedParameters, _lockModes, selection, _readOnly, _cacheable, _cacheRegion, _comment,
				_collectionKeys, _optionalObject, _optionalEntityName, _optionalId, _resultTransformer);
			copy.processedSQL = processedSQL;
			copy.processedPositionalParameterTypes = processedPositionalParameterTypes;
			copy.processedPositionalParameterValues = processedPositionalParameterValues;
			return copy;
		}
	}
}
