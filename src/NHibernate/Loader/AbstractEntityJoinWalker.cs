using System.Collections;
using System.Collections.Generic;
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
		private string alias;

		public AbstractEntityJoinWalker(IOuterJoinLoadable persister, ISessionFactoryImplementor factory,
																		IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.persister = persister;
			alias = GenerateRootAlias(persister.MappedClass.FullName);
		}

		public AbstractEntityJoinWalker(string alias, IOuterJoinLoadable persister, ISessionFactoryImplementor factory,
																		IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.persister = persister;
			this.alias = alias;
		}

		protected void InitAll(
			SqlString whereString,
			string orderByString,
			LockMode lockMode)
		{
			WalkEntityTree(persister, Alias);
			IList allAssociations = new ArrayList();
			foreach (object obj in associations)
			{
				allAssociations.Add(obj);
			}
			allAssociations.Add(
				new OuterJoinableAssociation(
					persister.EntityType,
					null,
					null,
					alias,
					JoinType.LeftOuterJoin,
					Factory,
					new CollectionHelper.EmptyMapClass<string, IFilter>()
					));

			InitPersisters(allAssociations, lockMode);
			InitStatementString(whereString, orderByString, lockMode);
		}

		protected void InitProjection(
			string projectionString,
			SqlString whereString,
			string orderByString,
			string groupByString,
			LockMode lockMode)

		{
			WalkEntityTree(persister, Alias);
			Persisters = new ILoadable[0];
			InitStatementString(projectionString, whereString, orderByString, groupByString, lockMode);
		}

		private void InitStatementString(
			SqlString condition,
			string orderBy,
			LockMode lockMode)

		{
			InitStatementString(null, condition, orderBy, "", lockMode);
		}

		private void InitStatementString(
			string projection,
			SqlString condition,
			string orderBy,
			string groupBy,
			LockMode lockMode)

		{
			int joins = CountEntityPersisters(associations);
			Suffixes = BasicLoader.GenerateSuffixes(joins + 1);
			JoinFragment ojf = MergeOuterJoins(associations);

			SqlSelectBuilder select = new SqlSelectBuilder(Factory)
				.SetLockMode(lockMode)
				.SetSelectClause(
				projection == null ?
				persister.SelectFragment(alias, Suffixes[joins]) + SelectString(associations) :
				projection
				)
				.SetFromClause(
				Dialect.AppendLockHint(lockMode, persister.FromTableFragment(alias)) +
				persister.FromJoinFragment(alias, true, true)
				)
				.SetWhereClause(condition)
				.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString + WhereFragment
				)
				.SetOrderByClause(OrderBy(associations, orderBy))
				.SetGroupByClause(groupBy);

			// TODO H3:
//			if( Factory.IsCommentsEnabled )
//			{
//				select.SetComment( Comment );
//			}

			SqlString = select.ToSqlString();
		}

		/// <summary>
		/// Don't bother with the discriminator, unless overridden by subclass
		/// </summary>
		protected virtual SqlString WhereFragment
		{
			get { return persister.WhereJoinFragment(alias, true, true); }
		}

		/// <summary>
		/// The superclass deliberately excludes collections
		/// </summary>
		protected override bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config,
		                                             Cascades.CascadeStyle cascadeStyle)
		{
			return IsJoinedFetchEnabledInMapping(config, type);
		}

		public abstract string Comment { get; }

		protected ILoadable Persister
		{
			get { return persister; }
		}

		protected string Alias
		{
			get { return alias; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + Persister.ClassName + ')';
		}
	}
}