using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using NHibernate.Hql.Classic;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
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
		public delegate int[] GetNamedParameterLocations(string parameterName);

		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (QueryParameters));

		private IType[] _positionalParameterTypes;
		private object[] _positionalParameterValues;
		private int[] _positionalParameterLocations;
		private IDictionary<string, TypedValue> _namedParameters;
		private IDictionary<string, LockMode> _lockModes;
		private IList<IType> filteredParameterTypes;
		private IList<object> filteredParameterValues;
		private IList<int> filteredParameterLocations;
		private RowSelection _rowSelection;
		private bool _cacheable;
		private string _cacheRegion;
		private object[] _collectionKeys;
		private object _optionalObject;
		private string _optionalEntityName;
		private object _optionalId;
		private string _comment;
		private bool _readOnly;
		private int? limitParameterIndex = null;
		private int? offsetParameterIndex = null;
		private int wildcardSubqueryLimitParameterIndex = -1;
		private IDictionary<int, int> _adjustedParameterLocations;
		private IDictionary<int, int> _tempPagingParameterIndexes;
		private IDictionary<int, int> _pagingParameterIndexMap;

		private SqlString processedSQL;

		private readonly IResultTransformer _resultTransformer;
		// not implemented: private ScrollMode _scrollMode;

		public QueryParameters() : this(ArrayHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray) {}

		public QueryParameters(IType type, object value) : this(new[] {type}, new[] {value}) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object optionalObject,
		                       string optionalEntityName, object optionalObjectId)
			: this(positionalParameterTypes, postionalParameterValues)
		{
			_optionalObject = optionalObject;
			_optionalId = optionalObjectId;
			_optionalEntityName = optionalEntityName;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues)
			: this(positionalParameterTypes, postionalParameterValues, null, null, false, null, null, false, null, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, null, collectionKeys) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues,
		                       IDictionary<string, TypedValue> namedParameters, object[] collectionKeys)
			: this(
				positionalParameterTypes, postionalParameterValues, namedParameters, null, null, false, false, null, null,
				collectionKeys, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues,
		                       IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool cacheable,
		                       string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer, IDictionary<int,int> tempPagingParameterIndexes)
			: this(
				positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection, false, cacheable, cacheRegion,
				comment, null, transformer)
		{
			NaturalKeyLookup = isLookupByNaturalKey;
			_tempPagingParameterIndexes = tempPagingParameterIndexes;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues,
		                       IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes,
		                       RowSelection rowSelection, bool readOnly, bool cacheable, string cacheRegion, string comment,
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
		                       IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes,
		                       RowSelection rowSelection, bool readOnly, bool cacheable, string cacheRegion, string comment,
		                       object[] collectionKeys, object optionalObject, string optionalEntityName, object optionalId,
		                       IResultTransformer transformer)
			: this(
				positionalParameterTypes, positionalParameterValues, namedParameters, lockModes, rowSelection, readOnly, cacheable,
				cacheRegion, comment, collectionKeys, transformer)
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

		public int? LimitParameterIndex
		{
			get { return limitParameterIndex; }
		}

		public int? OffsetParameterIndex
		{
			get { return offsetParameterIndex; }
		}

		/// <summary>
		/// Named parameters.
		/// </summary>
		public IDictionary<string, TypedValue> NamedParameters
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

		public int[] PositionalParameterLocations
		{
			get { return _positionalParameterLocations; }
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
		public IDictionary<string, LockMode> LockModes
		{
			get { return _lockModes; }
			set { _lockModes = value; }
		}

		private void CreatePositionalParameterLocations(ISessionFactoryImplementor factory)
		{
			_positionalParameterLocations = new int[_positionalParameterTypes.Length];
			int location = 0;
			for (int i = 0; i < _positionalParameterLocations.Length; i++)
			{
				var span = _positionalParameterTypes[i].GetColumnSpan(factory);
				_positionalParameterLocations[i] = location;
				location += span;
			}
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
			var print = new Printer(factory);
			if (_positionalParameterValues.Length != 0)
			{
				log.Debug("parameters: " + print.ToString(_positionalParameterTypes, _positionalParameterValues));
			}

			if (_namedParameters != null)
			{
				log.Debug("named parameters: " + print.ToString(_namedParameters));
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

		public string Comment
		{
			get { return _comment; }
			set { _comment = value; }
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
				throw new QueryException("Number of positional parameter types (" + typesLength
				                         + ") does not match number of positional parameter values (" + valuesLength + ")");
			}
		}

		public bool ForceCacheRefresh { get; set; }

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

		public bool Callable { get; set; }

		public bool ReadOnly
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}

		/************** Filters ********************************/

		public int FindAdjustedParameterLocation(int parameterIndex)
		{
			if (_adjustedParameterLocations == null)
				return parameterIndex;

			return _adjustedParameterLocations[parameterIndex];
		}

		public void ProcessFilters(SqlString sql, ISessionImplementor session)
		{
			filteredParameterValues = new List<object>();
			filteredParameterTypes = new List<IType>();
			filteredParameterLocations = new List<int>();

			if (session.EnabledFilters.Count == 0 || sql.ToString().IndexOf(ParserHelper.HqlVariablePrefix) < 0)
			{
				processedSQL = sql.Copy();
				return;
			}

			Dialect.Dialect dialect = session.Factory.Dialect;
			string symbols = ParserHelper.HqlSeparators + dialect.OpenQuote + dialect.CloseQuote;

			var result = new SqlStringBuilder();

			int originalParameterIndex = 0; // keep track of the positional parameter
			int newParameterIndex = 0;
			_adjustedParameterLocations = new Dictionary<int, int>();

			foreach (var part in sql.Parts)
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

					_adjustedParameterLocations[originalParameterIndex] = newParameterIndex;
					originalParameterIndex++;
					newParameterIndex++;
					continue;
				}

				var tokenizer = new StringTokenizer((string) part, symbols, true);

				foreach (var token in tokenizer)
				{
					if (token.StartsWith(ParserHelper.HqlVariablePrefix))
					{
						string filterParameterName = token.Substring(1);
						object value = session.GetFilterParameterValue(filterParameterName);
						IType type = session.GetFilterParameterType(filterParameterName);

						// If the value is not a value of the type but a collection of values...
						if (value != null && !type.ReturnedClass.IsAssignableFrom(value.GetType()) && // Added to fix NH-882
						    typeof (ICollection).IsAssignableFrom(value.GetType()))
						{
							var coll = (ICollection) value;
							int i = 0;
							foreach (var elementValue in coll)
							{
								i++;
								int span = type.GetColumnSpan(session.Factory);
								if (span > 0)
								{
									result.AddParameter();
									filteredParameterTypes.Add(type);
									filteredParameterValues.Add(elementValue);
									filteredParameterLocations.Add(newParameterIndex);
									newParameterIndex++;
									if (i < coll.Count)
									{
										result.Add(", ");
									}
								}
							}
						}
						else
						{
							int span = type.GetColumnSpan(session.Factory);
							if (span > 0)
							{
								result.AddParameter();
								filteredParameterTypes.Add(type);
								filteredParameterValues.Add(value);
								filteredParameterLocations.Add(newParameterIndex);
								newParameterIndex++;
							}
						}
					}
					else
					{
						result.Add(token);
					}
				}
			}

			processedSQL = result.ToSqlString();
		}

		private IList<Parameter> FindParametersIn(SqlString sqlString)
		{
			IList<Parameter> sqlParameters = new List<Parameter>();

			foreach (object sqlParameter in sqlString.Parts)
			{
				if (sqlParameter is Parameter)
				{
					var parameter = (Parameter) sqlParameter;
					if (!parameter.ParameterPosition.HasValue || (parameter.ParameterPosition >= 0))
					{
						sqlParameters.Add(parameter);
					}
				}
			}

			return sqlParameters;
		}

		private void SetParameterLocation(IList<Parameter> sqlParameters, int parameterIndex, int sqlLocation, int span)
		{
			int i = 0;
			while (i < span)
			{
				sqlParameters[sqlLocation + i].ParameterPosition = parameterIndex + i;
				i++;
			}
		}

		private SqlType[] ConvertITypesToSqlTypes(List<IType> nhTypes, ISessionFactoryImplementor factory, int totalSpan)
		{
			SqlType[] result = new SqlType[totalSpan];

			int index = 0;
			foreach (IType type in nhTypes)
			{
				int span = type.SqlTypes(factory).Length;
				Array.Copy(type.SqlTypes(factory), 0, result, index, span);
				index += span;
			}

			return result;
		}

		public SqlType[] PrepareParameterTypes(SqlString sqlString, ISessionFactoryImplementor factory, GetNamedParameterLocations getNamedParameterLocations, int startParameterIndex, bool addLimit, bool addOffset)
		{
			List<IType> paramTypeList = new List<IType>();
			int parameterIndex = 0;
			int totalSpan = 0;

			CreatePositionalParameterLocations(factory);

			IList<Parameter> sqlParameters = FindParametersIn(sqlString);

			for (int index = 0; index < PositionalParameterTypes.Length; index++)
			{
				IType type = PositionalParameterTypes[index];
				ArrayHelper.SafeSetValue(paramTypeList, parameterIndex, type);

				int location = PositionalParameterLocations[index];
				location = FindAdjustedParameterLocation(location);
				int span = type.GetColumnSpan(factory);
				SetParameterLocation(sqlParameters, startParameterIndex + totalSpan, location, span);

				totalSpan += span;
				parameterIndex++;
			}

			for (int index = 0; index < FilteredParameterTypes.Count; index++)
			{
				IType type = FilteredParameterTypes[index];
				ArrayHelper.SafeSetValue(paramTypeList, parameterIndex, type);

				int location = FilteredParameterLocations[index];
				int span = type.GetColumnSpan(factory);
				SetParameterLocation(sqlParameters, startParameterIndex + totalSpan, location, span);

				totalSpan += span;
				parameterIndex++;
			}

			if (NamedParameters != null && NamedParameters.Count > 0)
			{
				// convert the named parameters to an array of types
				foreach (KeyValuePair<string, TypedValue> namedParameter in NamedParameters)
				{
					TypedValue typedval = namedParameter.Value;
					ArrayHelper.SafeSetValue(paramTypeList, parameterIndex, typedval.Type);

					int span = typedval.Type.GetColumnSpan(factory);
					string name = namedParameter.Key;
					int[] locs = getNamedParameterLocations(name);
					for (int i = 0; i < locs.Length; i++)
					{
						int location = locs[i];
						location = FindAdjustedParameterLocation(location);

						// can still clash with positional parameters
						//  could consider throwing an exception to locate problem (NH-1098)
						while ((location < sqlParameters.Count) && (sqlParameters[location].ParameterPosition != null))
							location++;

						SetParameterLocation(sqlParameters, startParameterIndex + totalSpan, location, span);
					}

					totalSpan += span;
					parameterIndex++;
				}
			}

			if (_tempPagingParameterIndexes != null)
			{
				_pagingParameterIndexMap = new Dictionary<int, int>();

				var pagingParameters =
					sqlString.Parts
						.Cast<object>()
						.Where(p => p is Parameter)
						.Cast<Parameter>()
						.Where(p => p.ParameterPosition.HasValue && p.ParameterPosition < 0)
						.ToList();

				foreach (Parameter pagingParameter in pagingParameters)
				{
					int pagingValue = _tempPagingParameterIndexes[pagingParameter.ParameterPosition.Value];
					int position = parameterIndex + startParameterIndex;
					_pagingParameterIndexMap.Add(position, pagingValue);
					pagingParameter.ParameterPosition = position;
					paramTypeList.Add(NHibernateUtil.Int32);
					parameterIndex++;
					totalSpan++;
				}
			}

			if (addLimit && factory.Dialect.SupportsVariableLimit)
			{
				if (factory.Dialect.BindLimitParametersFirst)
				{
					paramTypeList.Insert(0, NHibernateUtil.Int32);
					limitParameterIndex = startParameterIndex - 1;
					if (addOffset)
					{
						paramTypeList.Insert(0, NHibernateUtil.Int32);
						offsetParameterIndex = startParameterIndex - 2;
					}
				}
				else
				{
					paramTypeList.Add(NHibernateUtil.Int32);
					limitParameterIndex = totalSpan;
					if (addOffset)
					{
						paramTypeList.Add(NHibernateUtil.Int32);
						offsetParameterIndex = totalSpan;
						limitParameterIndex = totalSpan + 1;
					}
				}

				if (addOffset && factory.Dialect.BindLimitParametersInReverseOrder)
				{
					int? temp = limitParameterIndex;
					limitParameterIndex = offsetParameterIndex;
					offsetParameterIndex = temp;
				}

				totalSpan += addOffset ? 2 : 1;
			}

			return ConvertITypesToSqlTypes(paramTypeList, factory, totalSpan);
		}

		public int BindParameters(IDbCommand command, int start, ISessionImplementor session)
		{
			int location = start;
			var values = new List<object>();
			var types = new List<IType>();
			var sources = new List<string>();

			for (int i = 0; i < _positionalParameterLocations.Length; i++)
			{
				object value = _positionalParameterValues[i];
				IType type = _positionalParameterTypes[i];
				ArrayHelper.SafeSetValue(values, location, value);
				ArrayHelper.SafeSetValue(types, location, type);
				ArrayHelper.SafeSetValue(sources, location, "Positional" + i);
				location++;
			}

			for (int i = 0; i < filteredParameterLocations.Count; i++)
			{
				object value = filteredParameterValues[i];
				IType type = filteredParameterTypes[i];
				ArrayHelper.SafeSetValue(values, location, value);
				ArrayHelper.SafeSetValue(types, location, type);
				ArrayHelper.SafeSetValue(sources, location, "Filter" + i);
				location++;
			}

			if ((_namedParameters != null) && (_namedParameters.Count > 0))
			{
				foreach (var namedParameter in _namedParameters)
				{
					string name = namedParameter.Key;
					TypedValue typedval = namedParameter.Value;
					ArrayHelper.SafeSetValue(values, location, typedval.Value);
					ArrayHelper.SafeSetValue(types, location, typedval.Type);
					ArrayHelper.SafeSetValue(sources, location, "name_" + name);
					location++;
				}
			}

			if (_pagingParameterIndexMap != null)
			{
				foreach (int pagingParameterIndex in _pagingParameterIndexMap.Keys)
				{
					ArrayHelper.SafeSetValue(values, pagingParameterIndex, _pagingParameterIndexMap[pagingParameterIndex]);
					ArrayHelper.SafeSetValue(types, pagingParameterIndex, NHibernateUtil.Int32);
					ArrayHelper.SafeSetValue(sources, pagingParameterIndex, "limit_" + pagingParameterIndex);
				}
			}

			int span = 0;
			for (int i = start; i < values.Count; i++)
			{
				IType type = types[i];
				object value = values[i];
				string source = sources[i];
				if (log.IsDebugEnabled)
				{
					log.Debug(string.Format("BindParameters({0}:{1}) {2} -> [{3}]", source, type, value, i));
				}
				type.NullSafeSet(command, value, start + span, session);
				span += type.GetColumnSpan(session.Factory);
			}

			return span;
		}

		public SqlString FilteredSQL
		{
			get { return processedSQL; }
		}

		public IList<IType> FilteredParameterTypes
		{
			get { return filteredParameterTypes; }
		}

		public IList<object> FilteredParameterValues
		{
			get { return filteredParameterValues; }
		}

		public IList<int> FilteredParameterLocations
		{
			get { return filteredParameterLocations; }
		}

		public bool NaturalKeyLookup { get; set; }

		public IResultTransformer ResultTransformer
		{
			get { return _resultTransformer; }
		}

		public bool HasAutoDiscoverScalarTypes { get; set; }

		public QueryParameters CreateCopyUsing(RowSelection selection)
		{
			var copy = new QueryParameters(_positionalParameterTypes, _positionalParameterValues, _namedParameters, _lockModes,
			                               selection, _readOnly, _cacheable, _cacheRegion, _comment, _collectionKeys,
			                               _optionalObject, _optionalEntityName, _optionalId, _resultTransformer);
			copy._positionalParameterLocations = _positionalParameterLocations;
			copy.processedSQL = processedSQL;
			copy.filteredParameterTypes = filteredParameterTypes;
			copy.filteredParameterValues = filteredParameterValues;
			copy.filteredParameterLocations = filteredParameterLocations;
			return copy;
		}
	}
}