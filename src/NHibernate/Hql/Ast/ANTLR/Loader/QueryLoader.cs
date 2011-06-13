using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Classic;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR.Loader
{
	[CLSCompliant(false)]
	public class QueryLoader : BasicLoader
	{
		private readonly QueryTranslatorImpl _queryTranslator;

		private bool _hasScalars;
		private string[][] _scalarColumnNames;
		private IType[] _queryReturnTypes;
		private IResultTransformer _selectNewTransformer;
		private string[] _queryReturnAliases;
		private IQueryableCollection[] _collectionPersisters;
		private int[] _collectionOwners;
		private string[] _collectionSuffixes;
		private IQueryable[] _entityPersisters;
		private bool[] _entityEagerPropertyFetches;
		private string[] _entityAliases;
		private string[] _sqlAliases;
		private string[] _sqlAliasSuffixes;
		private bool[] _includeInSelect;
		private int[] _owners;
		private EntityType[] _ownerAssociationTypes;
		private readonly NullableDictionary<string, string> _sqlAliasByEntityAlias = new NullableDictionary<string, string>();
		private int _selectLength;
		private LockMode[] _defaultLockModes;

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
			Dictionary<string, string[]> keyColumnNames = dialect.ForUpdateOfColumns ? new Dictionary<string, string[]>() : null;

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
		/// Returns the locations of all occurrences of the named parameter.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override int[] GetNamedParameterLocs(string name)
		{
			return _queryTranslator.GetParameterTranslations().GetNamedParameterSqlLocations(name);
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

		private void Initialize(SelectClause selectClause)
		{
			IList<FromElement> fromElementList = selectClause.FromElementsForLoad;

			_hasScalars = selectClause.IsScalarSelect;
			_scalarColumnNames = selectClause.ColumnNames;
			//sqlResultTypes = selectClause.getSqlResultTypes();
			_queryReturnTypes = selectClause.QueryReturnTypes;

			_selectNewTransformer = HolderInstantiator.CreateSelectNewTransformer(selectClause.Constructor, selectClause.IsMap, selectClause.IsList);
			_queryReturnAliases = selectClause.QueryReturnAliases;

			IList<FromElement> collectionFromElements = selectClause.CollectionFromElements;
			if (collectionFromElements != null && collectionFromElements.Count != 0)
			{
				int length = collectionFromElements.Count;
				_collectionPersisters = new IQueryableCollection[length];
				_collectionOwners = new int[length];
				_collectionSuffixes = new string[length];

				for (int i = 0; i < length; i++)
				{
					FromElement collectionFromElement = collectionFromElements[i];
					_collectionPersisters[i] = collectionFromElement.QueryableCollection;
					_collectionOwners[i] = fromElementList.IndexOf(collectionFromElement.Origin);
					//				collectionSuffixes[i] = collectionFromElement.getColumnAliasSuffix();
					//				collectionSuffixes[i] = Integer.toString( i ) + "_";
					_collectionSuffixes[i] = collectionFromElement.CollectionSuffix;
				}
			}

			int size = fromElementList.Count;
			_entityPersisters = new IQueryable[size];
			_entityEagerPropertyFetches = new bool[size];
			_entityAliases = new String[size];
			_sqlAliases = new String[size];
			_sqlAliasSuffixes = new String[size];
			_includeInSelect = new bool[size];
			_owners = new int[size];
			_ownerAssociationTypes = new EntityType[size];

			for (int i = 0; i < size; i++)
			{
				FromElement element = fromElementList[i];
				_entityPersisters[i] = (IQueryable) element.EntityPersister;

				if (_entityPersisters[i] == null)
				{
					throw new InvalidOperationException("No entity persister for " + element);
				}

				_entityEagerPropertyFetches[i] = element.IsAllPropertyFetch;
				_sqlAliases[i] = element.TableAlias;
				_entityAliases[i] = element.ClassAlias;
				_sqlAliasByEntityAlias.Add(_entityAliases[i], _sqlAliases[i]);
				// TODO should we just collect these like with the collections above?
				_sqlAliasSuffixes[i] = (size == 1) ? "" : i + "_";
				//			sqlAliasSuffixes[i] = element.getColumnAliasSuffix();
				_includeInSelect[i] = !element.IsFetch;
				if (_includeInSelect[i])
				{
					_selectLength++;
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
						var entityType = (EntityType) element.DataType;
						if (entityType.IsOneToOne)
						{
							_owners[i] = fromElementList.IndexOf(element.Origin);
						}
						_ownerAssociationTypes[i] = entityType;
					}
				}
			}

			//NONE, because its the requested lock mode, not the actual! 
			_defaultLockModes = ArrayHelper.FillArray(LockMode.None, size);
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			CheckQuery(queryParameters);
			return List(session, queryParameters, _queryTranslator.QuerySpaces, _queryReturnTypes);
		}

		public override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			// meant to handle dynamic instantiation queries...
			HolderInstantiator holderInstantiator = HolderInstantiator.GetHolderInstantiator(_selectNewTransformer,
			                                                                                 resultTransformer,
			                                                                                 _queryReturnAliases);
			if (holderInstantiator.IsRequired)
			{
				for (int i = 0; i < results.Count; i++)
				{
					var row = (Object[]) results[i];
					Object result = holderInstantiator.Instantiate(row);
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

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			row = ToResultRow(row);
			bool hasTransform = HasSelectNew || resultTransformer != null;

			if (_hasScalars)
			{
				string[][] scalarColumns = _scalarColumnNames;
				int queryCols = _queryReturnTypes.Length;

				if (!hasTransform && queryCols == 1)
				{
					return _queryReturnTypes[0].NullSafeGet(rs, scalarColumns[0], session, null);
				}
				else
				{
					row = new object[queryCols];
					for (int i = 0; i < queryCols; i++)
					{
						row[i] = _queryReturnTypes[i].NullSafeGet(rs, scalarColumns[i], session, null);
					}
					return row;
				}
			}
			else if (!hasTransform)
			{
				return row.Length == 1 ? row[0] : row;
			}
			else
			{
				return row;
			}
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

		public IType[] ReturnTypes
		{
			get { return _queryReturnTypes; }
		}

		internal IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session)
		{
			CheckQuery(queryParameters);
			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;

			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			IDbCommand cmd = PrepareQueryCommand(queryParameters, false, session);

			// This IDataReader is disposed of in EnumerableImpl.Dispose
			IDataReader rs = GetResultSet(cmd, queryParameters.HasAutoDiscoverScalarTypes, false, queryParameters.RowSelection, session);

			HolderInstantiator hi = 
				HolderInstantiator.GetHolderInstantiator(_selectNewTransformer, queryParameters.ResultTransformer, _queryReturnAliases);

			IEnumerable result = 
				new EnumerableImpl(rs, cmd, session, queryParameters.IsReadOnly(session), _queryTranslator.ReturnTypes, _queryTranslator.GetColumnNames(), queryParameters.RowSelection, hi);

			if (statsEnabled)
			{
				stopWath.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted("HQL: " + _queryTranslator.QueryString, 0, stopWath.Elapsed);
				// NH: Different behavior (H3.2 use QueryLoader in AST parser) we need statistic for orginal query too.
				// probably we have a bug some where else for statistic RowCount
				session.Factory.StatisticsImplementor.QueryExecuted(QueryIdentifier, 0, stopWath.Elapsed);
			}
			return result;
		}

		public override ISqlCommand CreateSqlCommand(QueryParameters queryParameters, ISessionImplementor session)
		{
			// A distinct-copy of parameter specifications collected during query construction
			var parameterSpecs = new HashSet<IParameterSpecification>(_queryTranslator.CollectedParameterSpecifications);
			SqlString sqlString = SqlString.Copy();

			// dynamic-filter parameters: during the HQL->SQL parsing, filters can be added as SQL_TOKEN/string and the SqlGenerator will not find it
			sqlString = ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
			AdjustQueryParametersForSubSelectFetching(sqlString, parameterSpecs, session, queryParameters); // NOTE: see TODO below

			sqlString = AddLimitsParametersIfNeeded(sqlString, parameterSpecs, queryParameters, session);
			// TODO: for sub-select fetching we have to try to assign the QueryParameter.ProcessedSQL here (with limits) but only after use IParameterSpecification for any kind of queries

			// The PreprocessSQL method can modify the SqlString but should never add parameters (or we have to override it)
			sqlString = PreprocessSQL(sqlString, queryParameters, session.Factory.Dialect);

			// After the last modification to the SqlString we can collect all parameters types.
			parameterSpecs.ResetEffectiveExpectedType(queryParameters);

			return new SqlCommandImpl(sqlString, parameterSpecs, queryParameters, session.Factory);
		}

		/// <summary>
		/// Obtain an <c>IDbCommand</c> with all parameters pre-bound. Bind positional parameters,
		/// named parameters, and limit parameters.
		/// </summary>
		/// <remarks>
		/// Creates an IDbCommand object and populates it with the values necessary to execute it against the 
		/// database to Load an Entity.
		/// </remarks>
		/// <param name="queryParameters">The <see cref="QueryParameters"/> to use for the IDbCommand.</param>
		/// <param name="scroll">TODO: find out where this is used...</param>
		/// <param name="session">The SessionImpl this Command is being prepared in.</param>
		/// <returns>A CommandWrapper wrapping an IDbCommand that is ready to be executed.</returns>
		protected internal override IDbCommand PrepareQueryCommand(QueryParameters queryParameters, bool scroll, ISessionImplementor session)
		{
			var sqlCommand = (SqlCommandImpl)CreateSqlCommand(queryParameters, session);
			var parameterSpecs = sqlCommand.Specifications;
			var sqlString = sqlCommand.Query;
			var sqlQueryParametersList = sqlCommand.SqlQueryParametersList;
			
			parameterSpecs.SetQueryParameterLocations(sqlQueryParametersList, session.Factory);

			IDbCommand command = session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, sqlCommand.ParameterTypes);

			try
			{
				RowSelection selection = queryParameters.RowSelection;
				if (selection != null && selection.Timeout != RowSelection.NoValue)
				{
					command.CommandTimeout = selection.Timeout;
				}

				sqlCommand.Bind(command, sqlQueryParametersList, 0, session);

				session.Batcher.ExpandQueryParameters(command, sqlString);
			}
			catch (HibernateException)
			{
				session.Batcher.CloseCommand(command, null);
				throw;
			}
			catch (Exception sqle)
			{
				session.Batcher.CloseCommand(command, null);
				ADOExceptionReporter.LogExceptions(sqle);
				throw;
			}
			return command;
		}

		private void AdjustQueryParametersForSubSelectFetching(SqlString sqlString, IEnumerable<IParameterSpecification> parameterSpecs, ISessionImplementor session, QueryParameters queryParameters)
		{
			// TODO: Remove this when all parameters are managed using IParameterSpecification (QueryParameters does not need to have decomposed values for filters)

			var dynamicFilterParameterSpecifications = parameterSpecs.OfType<DynamicFilterParameterSpecification>().ToList();
			var filteredParameterValues = new List<object>();
			var filteredParameterTypes = new List<IType>();
			var filteredParameterLocations = new List<int>();

			if (dynamicFilterParameterSpecifications.Count != 0)
			{
				var sqlQueryParametersList = sqlString.GetParameters().ToList();
				foreach (DynamicFilterParameterSpecification specification in dynamicFilterParameterSpecifications)
				{
					string backTrackId = specification.GetIdsForBackTrack(session.Factory).First();
					object value = session.GetFilterParameterValue(specification.FilterParameterFullName);
					var elementType = specification.ExpectedType;
					foreach (int position in sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
					{
						filteredParameterValues.Add(value);
						filteredParameterTypes.Add(elementType);
						filteredParameterLocations.Add(position);
					}
				}
			}

			queryParameters.ProcessedSql = sqlString;
			queryParameters.FilteredParameterLocations = filteredParameterLocations;
			queryParameters.FilteredParameterTypes = filteredParameterTypes;
			queryParameters.FilteredParameterValues = filteredParameterValues;
		}

		private SqlString ExpandDynamicFilterParameters(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, ISessionImplementor session)
		{
			var enabledFilters = session.EnabledFilters;
			if (enabledFilters.Count == 0 || sqlString.ToString().IndexOf(ParserHelper.HqlVariablePrefix) < 0)
			{
				return sqlString;
			}

			Dialect.Dialect dialect = session.Factory.Dialect;
			string symbols = ParserHelper.HqlSeparators + dialect.OpenQuote + dialect.CloseQuote;

			var originSql = sqlString.Compact();
			var result = new SqlStringBuilder();
			foreach (var sqlPart in originSql.Parts)
			{
				var parameter = sqlPart as Parameter;
				if (parameter != null)
				{
					result.Add(parameter);
					continue;
				}

				var sqlFragment = sqlPart.ToString();
				var tokens = new StringTokenizer(sqlFragment, symbols, true);

				foreach (string token in tokens)
				{
					if (token.StartsWith(ParserHelper.HqlVariablePrefix))
					{
						string filterParameterName = token.Substring(1);
						string[] parts = StringHelper.ParseFilterParameterName(filterParameterName);
						string filterName = parts[0];
						string parameterName = parts[1];
						var filter = (FilterImpl)enabledFilters[filterName];

						object value = filter.GetParameter(parameterName);
						IType type = filter.FilterDefinition.GetParameterType(parameterName);
						int parameterColumnSpan = type.GetColumnSpan(session.Factory);
						var collectionValue = value as ICollection;
						int? collectionSpan = null;

						// Add query chunk
						string typeBindFragment = string.Join(", ", Enumerable.Repeat("?", parameterColumnSpan).ToArray());
						string bindFragment;
						if (collectionValue != null && !type.ReturnedClass.IsArray)
						{
							collectionSpan = collectionValue.Count;
							bindFragment = string.Join(", ", Enumerable.Repeat(typeBindFragment, collectionValue.Count).ToArray());
						}
						else
						{
							bindFragment = typeBindFragment;
						}

						// dynamic-filter parameter tracking
						var filterParameterFragment = SqlString.Parse(bindFragment);
						var dynamicFilterParameterSpecification = new DynamicFilterParameterSpecification(filterName, parameterName, type, collectionSpan);
						var parameters = filterParameterFragment.GetParameters().ToArray();
						var sqlParameterPos = 0;
						var paramTrackers = dynamicFilterParameterSpecification.GetIdsForBackTrack(session.Factory);
						foreach (var paramTracker in paramTrackers)
						{
							parameters[sqlParameterPos++].BackTrack = paramTracker;
						}

						parameterSpecs.Add(dynamicFilterParameterSpecification);
						result.Add(filterParameterFragment);
					}
					else
					{
						result.Add(token);
					}
				}
			}
			return result.ToSqlString().Compact();
		}

		private SqlString AddLimitsParametersIfNeeded(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, QueryParameters queryParameters, ISessionImplementor session)
		{
			var sessionFactory = session.Factory;
			Dialect.Dialect dialect = sessionFactory.Dialect;

			RowSelection selection = queryParameters.RowSelection;
			bool useLimit = UseLimit(selection, dialect);
			if (useLimit)
			{
				bool hasFirstRow = GetFirstRow(selection) > 0;
				bool useOffset = hasFirstRow && dialect.SupportsLimitOffset;
				int max = GetMaxOrLimit(dialect, selection);
				int? skip = useOffset ? (int?) dialect.GetOffsetValue(GetFirstRow(selection)) : null;
				int? take = max != int.MaxValue ? (int?) max : null;

				Parameter skipSqlParameter = null;
				Parameter takeSqlParameter = null;
				if (skip.HasValue)
				{
					var skipParameter = new QuerySkipParameterSpecification();
					skipSqlParameter = Parameter.Placeholder;
					skipSqlParameter.BackTrack = skipParameter.GetIdsForBackTrack(sessionFactory).First();
					parameterSpecs.Add(skipParameter);
				}
				if (take.HasValue)
				{
					var takeParameter = new QueryTakeParameterSpecification();
					takeSqlParameter = Parameter.Placeholder;
					takeSqlParameter.BackTrack = takeParameter.GetIdsForBackTrack(sessionFactory).First();
					parameterSpecs.Add(takeParameter);
				}
				// The dialect can move the given parameters where he need, what it can't do is generates new parameters loosing the BackTrack.
				return dialect.GetLimitString(sqlString, skip, take, skipSqlParameter, takeSqlParameter);
			}
			return sqlString;
		}
	}
}