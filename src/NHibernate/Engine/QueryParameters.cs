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
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (QueryParameters));

		private bool readOnly;

		public QueryParameters() : this(TypeHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object optionalObject, string optionalEntityName, object optionalObjectId)
			: this(positionalParameterTypes, postionalParameterValues)
		{
			OptionalObject = optionalObject;
			OptionalId = optionalObjectId;
			OptionalEntityName = optionalEntityName;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues)
			: this(positionalParameterTypes, postionalParameterValues, null, null, false, false, false, null, null, false, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, null, collectionKeys) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] postionalParameterValues, IDictionary<string, TypedValue> namedParameters, object[] collectionKeys)
			: this(positionalParameterTypes, postionalParameterValues, namedParameters, null, null, false, false, false, null, null, collectionKeys, null) {}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, LockMode> lockModes, RowSelection rowSelection,
		                       bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer)
			: this(positionalParameterTypes, positionalParameterValues, null, lockModes, rowSelection, isReadOnlyInitialized, readOnly, cacheable, cacheRegion, comment, null, transformer)
		{
			NaturalKeyLookup = isLookupByNaturalKey;
		}

		public QueryParameters(IDictionary<string, TypedValue> namedParameters, IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized,
		                       bool readOnly, bool cacheable, string cacheRegion, string comment, bool isLookupByNaturalKey, IResultTransformer transformer)
			: this(
				TypeHelper.EmptyTypeArray, ArrayHelper.EmptyObjectArray, namedParameters, lockModes, rowSelection, isReadOnlyInitialized, readOnly, cacheable, cacheRegion, comment, null,
				transformer)
		{
			// used by CriteriaTranslator
			NaturalKeyLookup = isLookupByNaturalKey;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, TypedValue> namedParameters,
		                       IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion,
		                       string comment, object[] collectionKeys, IResultTransformer transformer)
		{
			PositionalParameterTypes = positionalParameterTypes ?? new IType[0];
			PositionalParameterValues = positionalParameterValues ?? new object[0];
			NamedParameters = namedParameters ?? new Dictionary<string, TypedValue>(1);
			LockModes = lockModes;
			RowSelection = rowSelection;
			Cacheable = cacheable;
			CacheRegion = cacheRegion;
			Comment = comment;
			CollectionKeys = collectionKeys;
			IsReadOnlyInitialized = isReadOnlyInitialized;
			this.readOnly = readOnly;
			ResultTransformer = transformer;
		}

		public QueryParameters(IType[] positionalParameterTypes, object[] positionalParameterValues, IDictionary<string, TypedValue> namedParameters,
		                       IDictionary<string, LockMode> lockModes, RowSelection rowSelection, bool isReadOnlyInitialized, bool readOnly, bool cacheable, string cacheRegion,
		                       string comment, object[] collectionKeys, object optionalObject, string optionalEntityName, object optionalId, IResultTransformer transformer)
			: this(
				positionalParameterTypes, positionalParameterValues, namedParameters, lockModes, rowSelection, isReadOnlyInitialized, readOnly, cacheable, cacheRegion, comment, collectionKeys,
				transformer)
		{
			OptionalEntityName = optionalEntityName;
			OptionalId = optionalId;
			OptionalObject = optionalObject;
		}

		public bool HasRowSelection
		{
			get { return RowSelection != null; }
		}

		public IDictionary<string, TypedValue> NamedParameters { get; internal set; }

		/// <summary>
		/// Gets or sets an array of <see cref="IType"/> objects that is stored at the index
		/// of the Parameter.
		/// </summary>
		public IType[] PositionalParameterTypes { get; set; }

		/// <summary>
		/// Gets or sets an array of <see cref="object"/> objects that is stored at the index
		/// of the Parameter.
		/// </summary>
		public object[] PositionalParameterValues { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="RowSelection"/> for the Query.
		/// </summary>
		public RowSelection RowSelection { get; set; }

		/// <summary>
		/// Gets or sets an <see cref="IDictionary"/> that contains the alias name of the
		/// object from hql as the key and the <see cref="LockMode"/> as the value.
		/// </summary>
		/// <value>An <see cref="IDictionary"/> of lock modes.</value>
		public IDictionary<string, LockMode> LockModes { get; set; }

		public bool IsReadOnlyInitialized { get; private set; }

		public bool Cacheable { get; set; }

		public string CacheRegion { get; set; }

		public string Comment { get; set; }

		public bool ForceCacheRefresh { get; set; }

		public string OptionalEntityName { get; set; }

		public object OptionalId { get; set; }

		public object OptionalObject { get; set; }

		public object[] CollectionKeys { get; set; }

		public bool Callable { get; set; }

		public bool ReadOnly
		{
			get
			{
				if (!IsReadOnlyInitialized)
				{
					throw new InvalidOperationException("cannot call ReadOnly when IsReadOnlyInitialized returns false");
				}

				return readOnly;
			}
			set
			{
				readOnly = value;
				IsReadOnlyInitialized = true;
			}
		}

		public SqlString ProcessedSql { get; internal set; }
		public IEnumerable<IParameterSpecification> ProcessedSqlParameters { get; internal set; }
		public RowSelection ProcessedRowSelection { get; internal set; }

		public bool NaturalKeyLookup { get; set; }

		public IResultTransformer ResultTransformer { get; private set; }

		public bool HasAutoDiscoverScalarTypes { get; set; }

		public void LogParameters(ISessionFactoryImplementor factory)
		{
			var print = new Printer(factory);
			if (PositionalParameterValues.Length != 0)
			{
				log.Debug("parameters: " + print.ToString(PositionalParameterTypes, PositionalParameterValues));
			}

			if (NamedParameters != null)
			{
				log.Debug("named parameters: " + print.ToString(NamedParameters));
			}
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
			int typesLength = PositionalParameterTypes == null ? 0 : PositionalParameterTypes.Length;
			int valuesLength = PositionalParameterValues == null ? 0 : PositionalParameterValues.Length;

			if (typesLength != valuesLength)
			{
				throw new QueryException("Number of positional parameter types (" + typesLength
				                         + ") does not match number of positional parameter values (" + valuesLength + ")");
			}
		}

		public QueryParameters CreateCopyUsing(RowSelection selection)
		{
			var copy = new QueryParameters(PositionalParameterTypes, PositionalParameterValues, NamedParameters, LockModes,
			                               selection, IsReadOnlyInitialized, readOnly, Cacheable, CacheRegion, Comment, CollectionKeys,
			                               OptionalObject, OptionalEntityName, OptionalId, ResultTransformer)
			           {
			           	ProcessedSql = ProcessedSql,
			            ProcessedSqlParameters = ProcessedSqlParameters != null ? ProcessedSqlParameters.ToList() : null
			           };
			return copy;
		}

		public bool IsReadOnly(ISessionImplementor session)
		{
			return IsReadOnlyInitialized ? ReadOnly : session.PersistenceContext.DefaultReadOnly;
		}
	}
}
