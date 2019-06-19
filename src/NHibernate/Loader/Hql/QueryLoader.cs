using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Impl;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Loader.Hql
{
	[CLSCompliant(false)]
	public partial class QueryLoader : BasicLoader
	{
		private readonly QueryTranslatorImpl _queryTranslator;

		private bool _hasScalars;
		private string[][] _scalarColumnNames;
		private IResultTransformer _selectNewTransformer;
		private string[] _queryReturnAliases;
		private IQueryableCollection[] _collectionPersisters;
		private int[] _collectionOwners;
		private string[] _collectionSuffixes;
		private IQueryable[] _entityPersisters;
		private bool[] _entityEagerPropertyFetches;
		private HashSet<string>[] _entityFetchLazyProperties;
		private string[] _entityAliases;
		private string[] _sqlAliases;
		private string[] _sqlAliasSuffixes;
		private bool[] _includeInSelect;
		private int[] _owners;
		private EntityType[] _ownerAssociationTypes;
		private readonly NullableDictionary<string, string> _sqlAliasByEntityAlias = new NullableDictionary<string, string>();
		private int _selectLength;
		private LockMode[] _defaultLockModes;
		private IType[] _cacheTypes;
		private ISet<ICollectionPersister> _uncacheableCollectionPersisters;
		private Dictionary<string, string[]>[] _collectionUserProvidedAliases;

		public QueryLoader(QueryTranslatorImpl queryTranslator, ISessionFactoryImplementor factory, SelectClause selectClause)
			: base(factory)
		{
			_queryTranslator = queryTranslator;

			Initialize(selectClause);
			PostInstantiate();
		}

		public override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		protected override SqlString ApplyLocks(SqlString sql, IDictionary<string, LockMode> lockModes,
												Dialect.Dialect dialect)
		{
			if (lockModes == null || lockModes.Count == 0)
			{
				return sql;
			}

			// can't cache this stuff either (per-invocation)
			// we are given a map of user-alias -> lock mode
			// create a new map of sql-alias -> lock mode
			var aliasedLockModes = new Dictionary<string, LockMode>();
			Dictionary<string, string[]> keyColumnNames = dialect.UsesColumnsWithForUpdateOf ? new Dictionary<string, string[]>() : null;

			foreach (var entry in lockModes)
			{
				string userAlias = entry.Key;
				string drivingSqlAlias = _sqlAliasByEntityAlias[userAlias];
				if (drivingSqlAlias == null)
				{
					throw new InvalidOperationException("could not locate alias to apply lock mode : " + userAlias);
				}

				// at this point we have (drivingSqlAlias) the SQL alias of the driving table
				// corresponding to the given user alias.  However, the driving table is not
				// (necessarily) the table against which we want to apply locks.  Mainly,
				// the exception case here is joined-subclass hierarchies where we instead
				// want to apply the lock against the root table (for all other strategies,
				// it just happens that driving and root are the same).
				var select = (QueryNode) _queryTranslator.SqlAST;
				var drivingPersister = (ILockable) select.FromClause.GetFromElement(userAlias).Queryable;
				string sqlAlias = drivingPersister.GetRootTableAlias(drivingSqlAlias);
				aliasedLockModes.Add(sqlAlias, entry.Value);

				if (keyColumnNames != null)
				{
					keyColumnNames.Add(sqlAlias, drivingPersister.RootTableIdentifierColumnNames);
				}
			}

			return dialect.ApplyLocksToSql(sql, aliasedLockModes, keyColumnNames);
		}

		protected override string[] Aliases
		{
			get { return _sqlAliases; }
		}

		protected override int[] CollectionOwners
		{
			get { return _collectionOwners; }
		}

		protected override bool[] EntityEagerPropertyFetches
		{
			get { return _entityEagerPropertyFetches; }
		}

		protected override HashSet<string>[] EntityFetchLazyProperties
		{
			get { return _entityFetchLazyProperties; }
		}

		protected override EntityType[] OwnerAssociationTypes
		{
			get { return _ownerAssociationTypes; }
		}

		protected override int[] Owners
		{
			get { return _owners; }
		}

		public override string QueryIdentifier
		{
			get { return _queryTranslator.QueryIdentifier; }
		}

		protected override bool UpgradeLocks()
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockModes">a collection of lock modes specified dynamically via the Query interface</param>
		/// <returns></returns>
		public override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
		{
			if (lockModes == null || lockModes.Count == 0)
			{
				return _defaultLockModes;
			}
			else
			{
				// unfortunately this stuff can't be cached because
				// it is per-invocation, not constant for the
				// QueryTranslator instance

				var lockModeArray = new LockMode[_entityAliases.Length];
				for (int i = 0; i < _entityAliases.Length; i++)
				{
					LockMode lockMode;

					if (!lockModes.TryGetValue(_entityAliases[i], out lockMode))
					{
						//NONE, because its the requested lock mode, not the actual! 
						lockMode = LockMode.None;
					}

					lockModeArray[i] = lockMode;
				}
				return lockModeArray;
			}
		}

		public override SqlString SqlString
		{
			get { return _queryTranslator.SqlString; }
		}

		public override ILoadable[] EntityPersisters
		{
			get { return _entityPersisters; }
		}

		protected override string[] Suffixes
		{
			get { return _sqlAliasSuffixes; }
		}

		protected override string[] CollectionSuffixes
		{
			get { return _collectionSuffixes; }
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return _collectionPersisters; }
		}

		public override IType[] CacheTypes => _cacheTypes;

		protected override IDictionary<string, string[]> GetCollectionUserProvidedAlias(int index)
		{
			return _collectionUserProvidedAliases?[index];
		}

		private void Initialize(SelectClause selectClause)
		{
			IList<FromElement> fromElementList = selectClause.FromElementsForLoad;

			_hasScalars = selectClause.IsScalarSelect;
			_scalarColumnNames = selectClause.ColumnNames;
			//sqlResultTypes = selectClause.getSqlResultTypes();
			ResultTypes = selectClause.QueryReturnTypes;

			_selectNewTransformer = GetSelectNewTransformer(selectClause);
			_queryReturnAliases = selectClause.QueryReturnAliases;

			IList<FromElement> collectionFromElements = selectClause.CollectionFromElements;
			if (collectionFromElements != null && collectionFromElements.Count != 0)
			{
				int length = collectionFromElements.Count;
				_collectionPersisters = new IQueryableCollection[length];
				_collectionOwners = new int[length];
				_collectionSuffixes = new string[length];
				CollectionFetches = new bool[length];
				if (collectionFromElements.Any(qc => qc.QueryableCollection.IsManyToMany))
					_collectionUserProvidedAliases = new Dictionary<string, string[]>[length];

				for (int i = 0; i < length; i++)
				{
					FromElement collectionFromElement = collectionFromElements[i];
					_collectionPersisters[i] = collectionFromElement.QueryableCollection;
					_collectionOwners[i] = fromElementList.IndexOf(collectionFromElement.Origin);
					//				collectionSuffixes[i] = collectionFromElement.getColumnAliasSuffix();
					//				collectionSuffixes[i] = Integer.toString( i ) + "_";
					_collectionSuffixes[i] = collectionFromElement.CollectionSuffix;
					CollectionFetches[i] = collectionFromElement.IsFetch;
				}
			}

			int size = fromElementList.Count;
			_entityPersisters = new IQueryable[size];
			_entityEagerPropertyFetches = new bool[size];
			_entityFetchLazyProperties = new HashSet<string>[size];
			_entityAliases = new String[size];
			_sqlAliases = new String[size];
			_sqlAliasSuffixes = new String[size];
			_includeInSelect = new bool[size];
			_owners = new int[size];
			_ownerAssociationTypes = new EntityType[size];
			EntityFetches = new bool[size];
			var cacheTypes = new List<IType>(ResultTypes);

			for (int i = 0; i < size; i++)
			{
				FromElement element = fromElementList[i];
				_entityPersisters[i] = (IQueryable) element.EntityPersister;

				if (_entityPersisters[i] == null)
				{
					throw new InvalidOperationException("No entity persister for " + element);
				}

				_entityEagerPropertyFetches[i] = element.IsAllPropertyFetch;
				_entityFetchLazyProperties[i] = element.FetchLazyProperties != null
					? new HashSet<string>(element.FetchLazyProperties)
					: null;
				_sqlAliases[i] = element.TableAlias;
				_entityAliases[i] = element.ClassAlias;
				_sqlAliasByEntityAlias.Add(_entityAliases[i], _sqlAliases[i]);
				// TODO should we just collect these like with the collections above?
				_sqlAliasSuffixes[i] = (size == 1) ? "" : i + "_";
				//			sqlAliasSuffixes[i] = element.getColumnAliasSuffix();
				_includeInSelect[i] = !element.IsFetch;
				EntityFetches[i] = element.IsFetch;
				if (element.IsFetch)
				{
					cacheTypes.Add(_entityPersisters[i].Type);
				}
				if (_includeInSelect[i])
				{
					_selectLength++;
				}

				if (collectionFromElements != null && element.IsFetch && element.QueryableCollection?.IsManyToMany == true
					&& element.QueryableCollection.IsManyToManyFiltered(_queryTranslator.EnabledFilters))
				{
					var collectionIndex = collectionFromElements.IndexOf(element);

					if (collectionIndex >= 0)
					{
						// When many-to-many is filtered we need to populate collection from element persister and not from bridge table.
						// As bridge table will contain not-null values for filtered elements
						// So do alias substitution for collection persister with element persister
						// See test TestFilteredLinqQuery for details
						_collectionUserProvidedAliases[collectionIndex] = new Dictionary<string, string[]>
						{
							{CollectionPersister.PropElement, _entityPersisters[i].GetIdentifierAliases(Suffixes[i])}
						};
					}
				}

				_owners[i] = -1; //by default
				if (element.IsFetch)
				{
					if (element.IsCollectionJoin || element.QueryableCollection != null)
					{
						// This is now handled earlier in this method.
					}
					else if (element.DataType.IsEntityType)
					{
						_owners[i] = fromElementList.IndexOf(element.Origin);
						_ownerAssociationTypes[i] = (EntityType) element.DataType;
					}
				}
			}

			if (_collectionPersisters != null)
			{
				cacheTypes.AddRange(_collectionPersisters.Where((t, i) => CollectionFetches[i]).Select(t => t.CollectionType));
			}

			_cacheTypes = cacheTypes.ToArray();

			//NONE, because its the requested lock mode, not the actual! 
			_defaultLockModes = ArrayHelper.Fill(LockMode.None, size);
			_uncacheableCollectionPersisters = _queryTranslator.UncacheableCollectionPersisters;
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			CheckQuery(queryParameters);
			return List(session, queryParameters, _queryTranslator.QuerySpaces);
		}

		public override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			// meant to handle dynamic instantiation queries...
			var transformer = _selectNewTransformer ?? resultTransformer;
			if (transformer != null)
			{
				for (int i = 0; i < results.Count; i++)
				{
					var row = (object[]) results[i];
					object result = transformer.TransformTuple(row, _queryReturnAliases);
					results[i] = result;
				}

				if (!HasSelectNew && resultTransformer != null)
				{
					return resultTransformer.TransformList(results);
				}
				else
				{
					return results;
				}
			}
			else
			{
				return results;
			}
		}

		protected override bool IsCollectionPersisterCacheable(ICollectionPersister collectionPersister)
		{
			return !_uncacheableCollectionPersisters.Contains(collectionPersister);
		}

		protected override IResultTransformer ResolveResultTransformer(IResultTransformer resultTransformer)
		{
			return _selectNewTransformer ?? resultTransformer;
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, DbDataReader rs,
													   ISessionImplementor session)
		{
			Object[] resultRow = GetResultRow(row, rs, session);
			bool hasTransform = HasSelectNew || resultTransformer != null;
			return (!hasTransform && resultRow.Length == 1
				        ? resultRow[0]
				        : resultRow
			       );
		}

		protected override object[] GetResultRow(object[] row, DbDataReader rs, ISessionImplementor session)
		{
			object[] resultRow;

			if (_hasScalars)
			{
				string[][] scalarColumns = _scalarColumnNames;
				int queryCols = ResultTypes.Length;
				resultRow = new object[queryCols];
				for (int i = 0; i < queryCols; i++)
				{
					resultRow[i] = ResultTypes[i].NullSafeGet(rs, scalarColumns[i], session, null);
				}
			}
			else
			{
				resultRow = ToResultRow(row);
			}

			return resultRow;
		}

		private object[] ToResultRow(object[] row)
		{
			if (_selectLength == row.Length)
			{
				return row;
			}

			var result = new object[_selectLength];
			int j = 0;
			for (int i = 0; i < row.Length; i++)
			{
				if (_includeInSelect[i])
				{
					result[j++] = row[i];
				}
			}
			return result;
		}

		private void CheckQuery(QueryParameters queryParameters)
		{
			if (HasSelectNew && queryParameters.ResultTransformer != null)
			{
				throw new QueryException("ResultTransformer is not allowed for 'select new' queries.");
			}
		}

		private bool HasSelectNew
		{
			get { return _selectNewTransformer != null; }
		}

		protected override string[] ResultRowAliases
		{
			get { return _queryReturnAliases; }
		}

		protected override bool[] IncludeInResultRow
		{
			get
			{
				bool[] includeInResultTuple = _includeInSelect;
				if (_hasScalars)
				{
					includeInResultTuple = new bool[ResultTypes.Length];
					ArrayHelper.Fill(includeInResultTuple, true);
				}
				return includeInResultTuple;
			}
		}

		[Obsolete("Please use ResultTypes instead")]
		public IType[] ReturnTypes => ResultTypes;

		internal IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session)
		{
			CheckQuery(queryParameters);
			Stopwatch stopWatch = null;
			if (session.Factory.Statistics.IsStatisticsEnabled)
			{
				stopWatch = Stopwatch.StartNew();
			}

			var cmd = PrepareQueryCommand(queryParameters, false, session);

			// This DbDataReader is disposed of in EnumerableImpl.Dispose
			var rs = GetResultSet(cmd, queryParameters, session, null);

			var resultTransformer = _selectNewTransformer ?? queryParameters.ResultTransformer;
			IEnumerable result = 
				new EnumerableImpl(rs, cmd, session, queryParameters.IsReadOnly(session), _queryTranslator.ReturnTypes, _queryTranslator.GetColumnNames(), queryParameters.RowSelection, resultTransformer, _queryReturnAliases);

			if (stopWatch != null)
			{
				stopWatch.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted("HQL: " + _queryTranslator.QueryString, 0, stopWatch.Elapsed);
				// NH: Different behavior (H3.2 use QueryLoader in AST parser) we need statistic for orginal query too.
				// probably we have a bug some where else for statistic RowCount
				session.Factory.StatisticsImplementor.QueryExecuted(QueryIdentifier, 0, stopWatch.Elapsed);
			}
			return result;
		}

		protected override void ResetEffectiveExpectedType(IEnumerable<IParameterSpecification> parameterSpecs, QueryParameters queryParameters)
		{
			parameterSpecs.ResetEffectiveExpectedType(queryParameters);
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return _queryTranslator.CollectedParameterSpecifications;
		}

		private static IResultTransformer GetSelectNewTransformer(SelectClause selectClause)
		{
			var constructor = selectClause.Constructor;
			if (constructor != null)
			{
				return new AliasToBeanConstructorResultTransformer(constructor);
			}

			if (selectClause.IsMap)
			{
				return Transformers.AliasToEntityMap;
			}

			if (selectClause.IsList)
			{
				return Transformers.ToList;
			}

			return null;
		}
	}
}
