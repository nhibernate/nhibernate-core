using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	public abstract class AbstractEntityJoinWalker : JoinWalker
	{
		private readonly IOuterJoinLoadable persister;
		private readonly string alias;

		public AbstractEntityJoinWalker(IOuterJoinLoadable persister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.persister = persister;
			alias = GenerateRootAlias(persister.EntityName);
		}

		public AbstractEntityJoinWalker(string rootSqlAlias, IOuterJoinLoadable persister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.persister = persister;
			alias = rootSqlAlias;
		}

		protected virtual void InitAll(SqlString whereString, SqlString orderByString, LockMode lockMode)
		{
			AddAssociations();
			ProcessJoins();
			IList<OuterJoinableAssociation> allAssociations = new List<OuterJoinableAssociation>(associations);
			var rootAssociation = CreateRootAssociation();
			allAssociations.Add(rootAssociation);

			InitPersisters(allAssociations, lockMode);
			InitStatementString(rootAssociation, null, whereString, orderByString, null, null, lockMode);
		}

		protected virtual OuterJoinableAssociation CreateRootAssociation()
		{
			return CreateAssociation(persister.EntityType, Alias);
		}

		//Since v5.1
		[Obsolete("Please use InitProjection(SqlString projectionString, SqlString whereString, SqlString orderByString, SqlString groupByString, SqlString havingString, IDictionary<string, IFilter> enabledFilters, LockMode lockMode, IList<EntityProjection> entityProjections) instead")]
		protected void InitProjection(SqlString projectionString, SqlString whereString, SqlString orderByString, SqlString groupByString, SqlString havingString, IDictionary<string, IFilter> enabledFilters, LockMode lockMode)
		{
			InitProjection(projectionString, whereString, orderByString, groupByString, havingString, enabledFilters, lockMode, Array.Empty<EntityProjection>());
		}

		protected void InitProjection(SqlString projectionString, SqlString whereString, SqlString orderByString, SqlString groupByString, SqlString havingString, IDictionary<string, IFilter> enabledFilters, LockMode lockMode, IList<EntityProjection> entityProjections)
		{
			AddAssociations();

			int countEntities = entityProjections.Count;
			if (countEntities > 0)
			{
				var associations = new OuterJoinableAssociation[countEntities];
				var eagerProps = new bool[countEntities];
				var suffixes = new string[countEntities];
				var fetchLazyProperties = new ISet<string>[countEntities];
				for (var i = 0; i < countEntities; i++)
				{
					var e = entityProjections[i];
					associations[i] = CreateAssociation(e.Persister.EntityMetamodel.EntityType, e.TableAlias);
					if (e.FetchLazyProperties)
					{
						eagerProps[i] = true;
					}

					if (e.FetchLazyPropertyGroups?.Count > 0)
					{
						fetchLazyProperties[i] = new HashSet<string>(e.FetchLazyPropertyGroups);
					}
					suffixes[i] = e.ColumnAliasSuffix;
				}

				InitPersisters(associations, lockMode);

				Suffixes = suffixes;
				EagerPropertyFetches = eagerProps;
				EntityFetchLazyProperties = fetchLazyProperties;
			}
			else
			{
				Persisters = Array.Empty<ILoadable>();
				Suffixes = Array.Empty<string>();
			}

			InitStatementString(null, projectionString, whereString, orderByString, groupByString, havingString, lockMode);
		}

		protected virtual void AddAssociations()
		{
			WalkEntityTree(persister, Alias);
		}

		private OuterJoinableAssociation CreateAssociation(EntityType entityType, string tableAlias)
		{
			return new OuterJoinableAssociation(
				entityType,
				null,
				null,
				tableAlias,
				JoinType.LeftOuterJoin,
				null,
				Factory,
				CollectionHelper.EmptyDictionary<string, IFilter>());
		}

		private void InitStatementString(OuterJoinableAssociation rootAssociation, SqlString projection, SqlString condition, SqlString orderBy, SqlString groupBy, SqlString having, LockMode lockMode)
		{
			SqlString selectClause = projection;
			if (selectClause == null)
			{
				int joins = CountEntityPersisters(associations);

				Suffixes = BasicLoader.GenerateSuffixes(joins + 1);
				var suffix = Suffixes[joins];
				selectClause = new SqlString(rootAssociation.GetSelectFragment(suffix, null, null) + SelectString(associations));
			}

			JoinFragment ojf = MergeOuterJoins(associations);

			SqlSelectBuilder select = new SqlSelectBuilder(Factory)
				.SetLockMode(lockMode, alias)
				.SetSelectClause(selectClause)
				.SetFromClause(Dialect.AppendLockHint(lockMode, persister.FromTableFragment(alias)) +persister.FromJoinFragment(alias, true, true))
				.SetWhereClause(condition)
				.SetOuterJoins(ojf.ToFromFragmentString,ojf.ToWhereFragmentString + WhereFragment)
				.SetOrderByClause(
					projection == null
						? OrderBy(associations, orderBy)
						: orderBy)
				.SetGroupByClause(groupBy)
				.SetHavingClause(having);

			if (Factory.Settings.IsCommentsEnabled)
				select.SetComment(Comment);

			SqlString = select.ToSqlString();
		}

		/// <summary>
		/// The superclass deliberately excludes collections
		/// </summary>
		protected override bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config, CascadeStyle cascadeStyle)
		{
			return IsJoinedFetchEnabledInMapping(config, type);
		}

		/// <summary>
		/// Don't bother with the discriminator, unless overridden by subclass
		/// </summary>
		protected virtual SqlString WhereFragment
		{
			get { return persister.WhereJoinFragment(alias, true, true); }
		}

		public abstract string Comment { get; }

		protected IOuterJoinLoadable Persister
		{
			get { return persister; }
		}

		protected string Alias
		{
			get { return alias; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + Persister.EntityName + ')';
		}
	}
}
