using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

		protected override IResultTransformer ResolveResultTransformer(IResultTransformer resultTransformer)
		{
			return translator.RootCriteria.ResultTransformer;
		}

		protected override bool AreResultSetRowsTransformedImmediately(IResultTransformer transformer)
		{
			// comparing to null just in case there is no transformer
			// (there should always be a result transformer; 
			return ResolveResultTransformer(transformer) != null;
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer customResultTransformer, IDataReader rs,
													   ISessionImplementor session)
		{
			object[] result;
			string[] aliases;

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
				aliases = translator.ProjectedAliases;
			}
			else
			{
				result = row;
				aliases = userAliases;
			}

			return ResolveResultTransformer(customResultTransformer).TransformTuple(result, aliases);
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