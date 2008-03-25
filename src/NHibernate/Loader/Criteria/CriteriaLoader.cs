using System.Collections;
using System.Data;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using System.Collections.Generic;
using Iesi.Collections.Generic;

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

		public CriteriaLoader(
			IOuterJoinLoadable persister,
			ISessionFactoryImplementor factory,
			CriteriaImpl rootCriteria,
			string rootEntityName, 
			IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			translator = new CriteriaQueryTranslator(
				factory,
				rootCriteria,
				rootEntityName,
				CriteriaQueryTranslator.RootSqlAlias);

			querySpaces = translator.GetQuerySpaces();

			CriteriaJoinWalker walker = new CriteriaJoinWalker(
				persister,
				translator,
				factory,
				rootCriteria,
				rootEntityName,
				enabledFilters);

			InitFromWalker(walker);

			userAliases = walker.UserAliases;
			resultTypes = walker.ResultTypes;

			PostInstantiate();
		}

		// Not ported: scroll (not supported)

		public IList List(ISessionImplementor session)
		{
			return List(session, translator.GetQueryParameters(), querySpaces, resultTypes);
		}


		public IType[] ResultTypes
		{
			get { return resultTypes; }
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

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}


		public CriteriaQueryTranslator Translator
		{
			get { return translator; }
		}

		protected override SqlString ApplyLocks(SqlString sqlSelectString, IDictionary lockModes, Dialect.Dialect dialect)
		{
			if (lockModes == null || lockModes.Count == 0)
			{
				return sqlSelectString;
			}

			IDictionary keyColumnNames = null;
			ILoadable[] persisters = EntityPersisters;
			string[] entityAliases = Aliases;

			if (dialect.ForUpdateOfColumns)
			{
				keyColumnNames = new Hashtable();
				for (int i = 0; i < entityAliases.Length; i++)
				{
					keyColumnNames[entityAliases[i]] = persisters[i].IdentifierColumnNames;
				}
			}

			return dialect.ApplyLocksToSql(sqlSelectString, lockModes, keyColumnNames);
		}

		protected internal override LockMode[] GetLockModes(IDictionary lockModes)
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
				LockMode lockMode = (LockMode) lockModes[entityAliases[i]];
				lockModesArray[i] = lockMode == null ? LockMode.None : lockMode;
			}
			return lockModesArray;
		}

		protected internal override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		protected override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			return translator.RootCriteria.ResultTransformer.TransformList(results);
		}
	}
}