using System.Collections;
using System.Collections.Generic;
using System.Data;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

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

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			object[] result;
			string[] aliases;

			if (translator.HasProjection)
			{
				IType[] types = translator.ProjectedTypes;
				result = new object[types.Length];
				string[] columnAliases = translator.ProjectedColumnAliases;

				for (int i = 0; i < result.Length; i++)
				{
					result[i] = types[i].NullSafeGet(rs, columnAliases[i], session, null);
				}
				aliases = translator.ProjectedAliases;
			}
			else
			{
				result = row;
				aliases = userAliases;
			}
			return translator.RootCriteria.ResultTransformer.TransformTuple(result, aliases);
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

		protected override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			return translator.RootCriteria.ResultTransformer.TransformList(results);
		}
	}
}