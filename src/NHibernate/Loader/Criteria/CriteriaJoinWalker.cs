using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Criteria
{
	/// <summary>
	/// A <see cref="JoinWalker" /> for <see cref="ICriteria" /> queries.
	/// </summary>
	public class CriteriaJoinWalker : AbstractEntityJoinWalker
	{
		//TODO: add a CriteriaImplementor interface
		//      this class depends directly upon CriteriaImpl in the impl package...

		private readonly CriteriaQueryTranslator translator;
		private readonly ISet<string> querySpaces;
		private readonly IType[] resultTypes;
		//the user visible aliases, which are unknown to the superclass,
		//these are not the actual "physical" SQL aliases
		private readonly string[] userAliases;
		private readonly IList userAliasList = new ArrayList();

		public IType[] ResultTypes
		{
			get { return resultTypes; }
		}

		public string[] UserAliases
		{
			get { return userAliases; }
		}

		public CriteriaJoinWalker(
			IOuterJoinLoadable persister,
			CriteriaQueryTranslator translator,
			ISessionFactoryImplementor factory,
			CriteriaImpl criteria,
			System.Type rootEntityName,
			IDictionary<string, IFilter> enabledFilters)
			: base(translator.RootSQLAlias, persister, factory, enabledFilters)
		{
			this.translator = translator;

			querySpaces = translator.GetQuerySpaces();

			if (translator.HasProjection)
			{
				resultTypes = translator.ProjectedTypes;

				InitProjection(
					translator.GetSelect(),
					translator.GetWhereCondition(enabledFilters),
					translator.GetOrderBy(),
					translator.GetGroupBy().ToString(),
					LockMode.None
					);
			}
			else
			{
				resultTypes = new IType[] {TypeFactory.ManyToOne(persister.EntityName)};

				InitAll(translator.GetWhereCondition(enabledFilters), translator.GetOrderBy(), LockMode.None);
			}

			userAliasList.Add(criteria.Alias); //root entity comes *last*
			userAliases = ArrayHelper.ToStringArray(userAliasList);
		}

		protected override JoinType GetJoinType(
			IAssociationType type,
			FetchMode config,
			string path,
			string lhsTable,
			string[] lhsColumns,
			bool nullable,
			int currentDepth,
			Cascades.CascadeStyle cascadeStyle)
		{
			if (translator.IsJoin(path))
			{
				return translator.GetJoinType(path);
			}
			else
			{
				if (translator.HasProjection)
				{
					return JoinType.None;
				}
				else
				{
					FetchMode fetchMode = translator.RootCriteria
						.GetFetchMode(path);
					if (IsDefaultFetchMode(fetchMode))
					{
						return base.GetJoinType(
							type,
							config,
							path,
							lhsTable,
							lhsColumns,
							nullable,
							currentDepth, cascadeStyle
							);
					}
					else
					{
						if (fetchMode == FetchMode.Join)
						{
							IsDuplicateAssociation(lhsTable, lhsColumns, type); //deliberately ignore return value!
							return GetJoinType(nullable, currentDepth);
						}
						else
						{
							return JoinType.None;
						}
					}
				}
			}
		}

		private static bool IsDefaultFetchMode(FetchMode fetchMode)
		{
			return fetchMode == FetchMode.Default;
		}

		/// <summary>
		/// Use the discriminator, to narrow the select to instances
		/// of the queried subclass, also applying any filters.
		/// </summary>
		protected override SqlString WhereFragment
		{
			get
			{
				return base.WhereFragment.Append(
					((IQueryable) Persister).FilterFragment(Alias, EnabledFilters));
			}
		}

		protected override string GenerateTableAlias(int n, string path, IJoinable joinable)
		{
			if (joinable.ConsumesEntityAlias())
			{
				ICriteria subcriteria = translator.GetCriteria(path);
				String sqlAlias = subcriteria == null ? null : translator.GetSQLAlias(subcriteria);
				if (sqlAlias != null)
				{
					userAliasList.Add(subcriteria.Alias); //alias may be null
					return sqlAlias; //EARLY EXIT
				}
				else
				{
					userAliasList.Add(null);
				}
			}
			return base.GenerateTableAlias(n + translator.SQLAliasCount, path, joinable);
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public override string Comment
		{
			get { return "criteria query"; }
		}
	}
}