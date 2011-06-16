using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Impl;
using NHibernate.Param;
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
		public delegate int[] GetNamedParameterLocations(string parameterName);

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (QueryParameters));

		private IType[] _positionalParameterTypes;
		private object[] _positionalParameterValues;
		private IDictionary<string, TypedValue> _namedParameters;
		private IDictionary<string, LockMode> _lockModes;
		private RowSelection _rowSelection;
		private bool _cacheable;
		private string _cacheRegion;
		private object[] _collectionKeys;
		private object _optionalObject;
		private string _optionalEntityName;
		private object _optionalId;
		private bool _isReadOnlyInitialized;
		private string _comment;
		private bool _readOnly;
		private SqlString processedSQL;
		private readonly IResultTransformer _resultTransformer;
		
		public QueryParameters() : this(ArrayHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object optionalObject, string optionalEntityName, object optionalObjectId)
			: this(positionalParameterTypes, postionalParameterValues)
		{
			_optionalObject = optionalObject;
			_optionalId = optionalObjectId;
			_optionalEntityName = optionalEntityName;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues)
			: this(positionalParameterTypes, postionalParameterValues, null, null, false, false, false, null, null, false, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, null, collectionKeys) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, IDictionary<string, TypedValue> namedParameters, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, namedParameters, null, null, false, false, false, null, null, collectionKeys, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer)
			: this(positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection, isReadOnlyInitialized, readOnly, cacheable, cacheRegion, comment, null, transformer)
		{
			NaturalKeyLookup = isLookupByNaturalKey;
		}

		public QueryParameters(IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer)
			: this(ArrayHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray, namedParameters, lockModes, rowSelection, isReadOnlyInitialized, readOnly, cacheable, cacheRegion, comment, null, transformer)
		{
			// used by CriteriaTranslator
			NaturalKeyLookup = isLookupByNaturalKey;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion, string comment, object[] collectionKeys, IResultTransformer transformer)
		{
			_positionalParameterTypes = positionalParameterTypes ?? new IType[0];
			_positionalParameterValues = positionalParameterValues ?? new IType[0];
			_namedParameters = namedParameters ?? new Dictionary<string, TypedValue>(1);
			_lockModes = lockModes;
			_rowSelection = rowSelection;
			_cacheable = cacheable;
			_cacheRegion = cacheRegion;
			_comment = comment;
			_collectionKeys = collectionKeys;
			_isReadOnlyInitialized = isReadOnlyInitialized;
			_readOnly = readOnly;
			_resultTransformer = transformer;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion, string comment, object[] collectionKeys, object optionalObject, string optionalEntityName, object optionalId, IResultTransformer transformer)
			: this(positionalParameterTypes, positionalParameterValues, namedParameters, lockModes, rowSelection, isReadOnlyInitialized, readOnly,  cacheable, cacheRegion, comment, collectionKeys, transformer)
		{
			_optionalEntityName = optionalEntityName;
			_optionalId = optionalId;
			_optionalObject = optionalObject;
		}

		public bool HasRowSelection
		{
			get { return _rowSelection != null; }
		}

		public IDictionary<string, TypedValue> NamedParameters
		{
			get { return _namedParameters; }
			internal set { _namedParameters = value; }
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
		public IDictionary<string, LockMode> LockModes
		{
			get { return _lockModes; }
			set { _lockModes = value; }
		}

		public bool IsReadOnlyInitialized
		{
			get { return _isReadOnlyInitialized; }
		}

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
			int typesLength = PositionalParameterTypes.Length;
			int valuesLength = PositionalParameterValues.Length;

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
			get
			{
				if (!_isReadOnlyInitialized)
					throw new InvalidOperationException("cannot call ReadOnly when IsReadOnlyInitialized returns false");

				return _readOnly;
			}
			set
			{
				_readOnly = value;
				_isReadOnlyInitialized = true;
			}
		}

		public SqlString ProcessedSql
		{
			get { return processedSQL; }
			internal set { processedSQL = value; }
		}

		public IEnumerable<IParameterSpecification> ProcessedSqlParameters { get; internal set; }

		public bool NaturalKeyLookup { get; set; }

		public IResultTransformer ResultTransformer
		{
			get { return _resultTransformer; }
		}

		public bool HasAutoDiscoverScalarTypes { get; set; }

		public QueryParameters CreateCopyUsing(RowSelection selection)
		{
			var copy = new QueryParameters(_positionalParameterTypes, _positionalParameterValues, _namedParameters, _lockModes,
			                               selection, _isReadOnlyInitialized, _readOnly, _cacheable, _cacheRegion, _comment, _collectionKeys,
			                               _optionalObject, _optionalEntityName, _optionalId, _resultTransformer);
			copy.processedSQL = processedSQL;
			copy.ProcessedSqlParameters = ProcessedSqlParameters.ToList();
			return copy;
		}
		
		public bool IsReadOnly(ISessionImplementor session)
		{
			return _isReadOnlyInitialized ? this.ReadOnly : session.PersistenceContext.DefaultReadOnly;
		}
	}
}