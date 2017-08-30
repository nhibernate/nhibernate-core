using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
	public partial class CriteriaLoader : OuterJoinLoader
	{
		private readonly CriteriaQueryTranslator translator;
		private readonly ISet<string> querySpaces;
		private readonly IType[] resultTypes;
		//the user visible aliases, which are unknown to the superclass,
		//these are not the actual "physical" SQL aliases
		private readonly string[] userAliases;
		private readonly bool[] includeInResultRow;
		private readonly int resultRowLength;
		// caching NH-3486
		private readonly string[] cachedProjectedColumnAliases;

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
			includeInResultRow = walker.IncludeInResultRow;
			resultRowLength = ArrayHelper.CountTrue(IncludeInResultRow);
			// fill caching objects only if there is a projection
			if (translator.HasProjection)
			{
				cachedProjectedColumnAliases = translator.ProjectedColumnAliases;
			}

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

		protected override string[] ResultRowAliases
		{
			get { return userAliases; }
		}

		protected override bool[] IncludeInResultRow
		{
			get { return includeInResultRow; }
		}

		public IList List(ISessionImplementor session)
		{
			return List(session, translator.GetQueryParameters(), querySpaces, resultTypes);
		}

		protected override IResultTransformer ResolveResultTransformer(IResultTransformer resultTransformer)
		{
			return translator.RootCriteria.ResultTransformer;
		}

		protected override bool AreResultSetRowsTransformedImmediately()
		{
			return true;
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer customResultTransformer, DbDataReader rs,
													   ISessionImplementor session)
		{
			return ResolveResultTransformer(customResultTransformer)
				.TransformTuple(GetResultRow(row, rs, session), ResultRowAliases);
		}


		protected override object[] GetResultRow(object[] row, DbDataReader rs, ISessionImplementor session)
		{
			object[] result;

			if (translator.HasProjection)
			{
				result = new object[ResultTypes.Length];

				for (int i = 0, position = 0; i < result.Length; i++)
				{
					int numColumns = ResultTypes[i].GetColumnSpan(session.Factory);

					if (numColumns > 1)
					{
						string[] typeColumnAliases = ArrayHelper.Slice(cachedProjectedColumnAliases, position, numColumns);
						result[i] = ResultTypes[i].NullSafeGet(rs, typeColumnAliases, session, null);
					}
					else
					{
						result[i] = ResultTypes[i].NullSafeGet(rs, cachedProjectedColumnAliases[position], session, null);
					}
					position += numColumns;
				}
			}
			else
			{
				result = ToResultRow(row);
			}
			return result;
		}


		private object[] ToResultRow(object[] row)
		{
			if (resultRowLength == row.Length)
				return row;

			var result = new object[resultRowLength];
			int j = 0;
			for (int i = 0; i < row.Length; i++)
			{
				if (includeInResultRow[i])
					result[j++] = row[i];
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

			//NH-3710: if we are issuing an aggregation function, Aliases will be null
			if (drivingSqlAliases != null)
			{
				for (int i = 0; i < drivingSqlAliases.Length; i++)
				{
					LockMode lockMode;
					if (lockModes.TryGetValue(drivingSqlAliases[i], out lockMode))
					{
						ILockable drivingPersister = (ILockable)EntityPersisters[i];
						string rootSqlAlias = drivingPersister.GetRootTableAlias(drivingSqlAliases[i]);
						aliasedLockModes[rootSqlAlias] = lockMode;
						if (keyColumnNames != null)
						{
							keyColumnNames[rootSqlAlias] = drivingPersister.RootTableIdentifierColumnNames;
						}
					}
				}
			}

			return dialect.ApplyLocksToSql(sqlSelectString, aliasedLockModes, keyColumnNames);
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

		public override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			return ResolveResultTransformer(resultTransformer).TransformList(results);
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return translator.CollectedParameterSpecifications;
		}
	}
}
