using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Classic;
using NHibernate.Impl;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Criteria
{
	/// <summary>
	/// A <c>Loader</c> for <see cref="ICriteria"/> queries. 
	/// </summary>
	/// <remarks>
	/// Note that criteria
	/// queries are more like multi-object <c>Load()</c>s than like HQL queries.
	/// </remarks>
	public class CriteriaLoader : OuterJoinLoader
	{
		private readonly CriteriaQueryTranslator translator;
		private readonly ISet<string> querySpaces;
		private readonly IType[] resultTypes;
		//the user visible aliases, which are unknown to the superclass,
		//these are not the actual "physical" SQL aliases
		private readonly string[] userAliases;

		public CriteriaLoader(IOuterJoinLoadable persister, ISessionFactoryImplementor factory, CriteriaImpl rootCriteria,
		                      string rootEntityName, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			translator = new CriteriaQueryTranslator(factory, rootCriteria, rootEntityName, CriteriaQueryTranslator.RootSqlAlias);

			querySpaces = translator.GetQuerySpaces();

			CriteriaJoinWalker walker =
				new CriteriaJoinWalker(persister, translator, factory, rootCriteria, rootEntityName, enabledFilters);

			InitFromWalker(walker);

			userAliases = walker.UserAliases;
			resultTypes = walker.ResultTypes;

			PostInstantiate();
		}

		// Not ported: scroll (not supported)

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		public CriteriaQueryTranslator Translator
		{
			get { return translator; }
		}

		public IType[] ResultTypes
		{
			get { return resultTypes; }
		}

		public IList List(ISessionImplementor session)
		{
			return List(session, translator.GetQueryParameters(), querySpaces, resultTypes);
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer customResultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			object[] result;

			if (translator.HasProjection)
			{
				IType[] types = translator.ProjectedTypes;
				result = new object[types.Length];
				string[] columnAliases = translator.ProjectedColumnAliases;
				
				for (int i = 0, position = 0; i < result.Length; i++)
				{
					int numColumns = types[i].GetColumnSpan(session.Factory);
					
					if ( numColumns > 1 ) 
					{
						string[] typeColumnAliases = ArrayHelper.Slice(columnAliases, position, numColumns);
						result[i] = types[i].NullSafeGet(rs, typeColumnAliases, session, null);
					}
					else
					{
						result[i] = types[i].NullSafeGet(rs, columnAliases[position], session, null);
					}
					position += numColumns;
				}
			}
			else
			{
				result = row;
			}

			if (customResultTransformer == null)
			{
				// apply the defaut transformer of criteria aka RootEntityResultTransformer
				return result[result.Length - 1];
			}
			return result;
		}

		protected override SqlString ApplyLocks(SqlString sqlSelectString, IDictionary<string, LockMode> lockModes,
		                                        Dialect.Dialect dialect)
		{
			if (lockModes == null || lockModes.Count == 0)
			{
				return sqlSelectString;
			}

			Dictionary<string, LockMode> aliasedLockModes = new Dictionary<string, LockMode>();
			Dictionary<string, string[]> keyColumnNames = dialect.ForUpdateOfColumns ? new Dictionary<string, string[]>() : null;
			string[] drivingSqlAliases = Aliases;
			for (int i = 0; i < drivingSqlAliases.Length; i++)
			{
				LockMode lockMode;
				if (lockModes.TryGetValue(drivingSqlAliases[i], out lockMode))
				{
					ILockable drivingPersister = (ILockable) EntityPersisters[i];
					string rootSqlAlias = drivingPersister.GetRootTableAlias(drivingSqlAliases[i]);
					aliasedLockModes[rootSqlAlias] = lockMode;
					if (keyColumnNames != null)
					{
						keyColumnNames[rootSqlAlias] = drivingPersister.RootTableIdentifierColumnNames;
					}
				}
			}

			return dialect.ApplyLocksToSql(sqlSelectString, lockModes, keyColumnNames);
		}

		public override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
		{
			string[] entityAliases = Aliases;
			if (entityAliases == null)
			{
				return null;
			}
			int size = entityAliases.Length;
			LockMode[] lockModesArray = new LockMode[size];
			for (int i = 0; i < size; i++)
			{
				LockMode lockMode;
				if (!lockModes.TryGetValue(entityAliases[i], out lockMode))
				{
					lockMode = LockMode.None;
				}
				lockModesArray[i] = lockMode;
			}
			return lockModesArray;
		}

		public override IList GetResultList(IList results, IResultTransformer customResultTransformer)
		{
			if (customResultTransformer == null)
			{
				// apply the defaut transformer of criteria aka RootEntityResultTransformer
				return results;
			}
			for (int i = 0; i < results.Count; i++)
			{
				var row = results[i] as object[] ?? new object[] { results[i] };
				object result = customResultTransformer.TransformTuple(row, translator.HasProjection ? translator.ProjectedAliases : userAliases);
				results[i] = result;
			}
			return customResultTransformer.TransformList(results);
		}

		public override ISqlCommand CreateSqlCommand(QueryParameters queryParameters, ISessionImplementor session)
		{
			// NOTE: repeated code PrepareQueryCommand
			// A distinct-copy of parameter specifications collected during query construction
			var parameterSpecs = new HashSet<IParameterSpecification>(translator.CollectedParameterSpecifications);
			SqlString sqlString = SqlString.Copy();

			// dynamic-filter parameters: during the HQL->SQL parsing, filters can be added as SQL_TOKEN/string and the SqlGenerator will not find it
			sqlString = ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
			AdjustQueryParametersForSubSelectFetching(sqlString, parameterSpecs, session, queryParameters); // NOTE: see TODO below

			sqlString = AddLimitsParametersIfNeeded(sqlString, parameterSpecs, queryParameters, session);
			// TODO: for sub-select fetching we have to try to assign the QueryParameter.ProcessedSQL here (with limits) but only after use IParameterSpecification for any kind of queries

			// The PreprocessSQL method can modify the SqlString but should never add parameters (or we have to override it)
			sqlString = PreprocessSQL(sqlString, queryParameters, session.Factory.Dialect);

			return new SqlCommand.SqlCommandImpl(sqlString, parameterSpecs, queryParameters, session.Factory);
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
			// NOTE: repeated code CreateSqlCommandInfo (here we are reusing some other variables)
			// A distinct-copy of parameter specifications collected during query construction
			var parameterSpecs = new HashSet<IParameterSpecification>(translator.CollectedParameterSpecifications);
			SqlString sqlString = SqlString.Copy();

			// dynamic-filter parameters: during the HQL->SQL parsing, filters can be added as SQL_TOKEN/string and the SqlGenerator will not find it
			sqlString = ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
			AdjustQueryParametersForSubSelectFetching(sqlString, parameterSpecs, session, queryParameters); // NOTE: see TODO below

			sqlString = AddLimitsParametersIfNeeded(sqlString, parameterSpecs, queryParameters, session);
			// TODO: for sub-select fetching we have to try to assign the QueryParameter.ProcessedSQL here (with limits) but only after use IParameterSpecification for any kind of queries

			// The PreprocessSQL method can modify the SqlString but should never add parameters (or we have to override it)
			sqlString = PreprocessSQL(sqlString, queryParameters, session.Factory.Dialect);

			// After the last modification to the SqlString we can collect all parameters.
			var sqlQueryParametersList = sqlString.GetParameters().ToList();
			SqlType[] parameterTypes = parameterSpecs.GetQueryParameterTypes(sqlQueryParametersList, session.Factory);

			parameterSpecs.SetQueryParameterLocations(sqlQueryParametersList, session.Factory);

			IDbCommand command = session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, parameterTypes);

			try
			{
				RowSelection selection = queryParameters.RowSelection;
				if (selection != null && selection.Timeout != RowSelection.NoValue)
				{
					command.CommandTimeout = selection.Timeout;
				}

				BindParametersValues(command, sqlQueryParametersList, parameterSpecs, queryParameters, session);

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

		public override int[] GetNamedParameterLocs(string name)
		{
			return new int[0];
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
				int? skip = useOffset ? (int?)dialect.GetOffsetValue(GetFirstRow(selection)) : null;
				int? take = max != int.MaxValue ? (int?)max : null;

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

		/// <summary>
		/// Bind all parameters values.
		/// </summary>
		/// <param name="command">The command where bind each value.</param>
		/// <param name="sqlQueryParametersList">The list of Sql query parameter in the exact sequence they are present in the query.</param>
		/// <param name="parameterSpecs">All parameter-specifications collected during query construction.</param>
		/// <param name="queryParameters">The encapsulation of the parameter values to be bound.</param>
		/// <param name="session">The session from where execute the query.</param>
		private void BindParametersValues(IDbCommand command, IList<Parameter> sqlQueryParametersList, IEnumerable<IParameterSpecification> parameterSpecs, QueryParameters queryParameters, ISessionImplementor session)
		{
			foreach (var parameterSpecification in parameterSpecs)
			{
				parameterSpecification.Bind(command, sqlQueryParametersList, queryParameters, session);
			}
		}
	}
}