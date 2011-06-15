using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
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
			// A distinct-copy of parameter specifications collected during query construction
			var parameterSpecs = new HashSet<IParameterSpecification>(translator.CollectedParameterSpecifications);
			SqlString sqlString = SqlString.Copy();

			// dynamic-filter parameters: during the Criteria parsing, filters can be added as string.
			sqlString = ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
			AdjustQueryParametersForSubSelectFetching(sqlString, parameterSpecs, session, queryParameters); // NOTE: see TODO below

			sqlString = AddLimitsParametersIfNeeded(sqlString, parameterSpecs, queryParameters, session);
			// TODO: for sub-select fetching we have to try to assign the QueryParameter.ProcessedSQL here (with limits) but only after use IParameterSpecification for any kind of queries

			// The PreprocessSQL method can modify the SqlString but should never add parameters (or we have to override it)
			sqlString = PreprocessSQL(sqlString, queryParameters, session.Factory.Dialect);

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
			var sqlCommand = CreateSqlCommand(queryParameters, session);
			var sqlString = sqlCommand.Query;

			sqlCommand.ResetParametersIndexesForTheCommand(0);
			IDbCommand command = session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, sqlCommand.ParameterTypes);

			try
			{
				RowSelection selection = queryParameters.RowSelection;
				if (selection != null && selection.Timeout != RowSelection.NoValue)
				{
					command.CommandTimeout = selection.Timeout;
				}

				sqlCommand.Bind(command, session);

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
	}
}