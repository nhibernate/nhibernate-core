using System;
using System.Text;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader 
{
	
	public class AbstractEntityLoader : OuterJoinLoader 
	{
		protected ILoadable persister;
		protected string alias;

		public AbstractEntityLoader(ILoadable persister, ISessionFactoryImplementor factory) : base(factory.Dialect) 
		{
			this.persister = persister;
			alias = Alias(persister.ClassName, 0);
		}

		protected void RenderStatement(SqlSelectBuilder selectBuilder, ISessionFactoryImplementor factory) 
		{
			RenderStatement(selectBuilder, String.Empty, factory);
		}

		protected void RenderStatement(SqlSelectBuilder selectBuilder, string orderBy, ISessionFactoryImplementor factory) 
		{
			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);

			selectBuilder.SetSelectClause(
				(joins==0 ? String.Empty : SelectString(associations) + ",") +
				SelectString(persister, alias, suffixes[joins] )
				)
				.SetFromClause
				(
					persister.FromTableFragment(alias) + 
					persister.FromJoinFragment(alias, true, true)
				)
				.SetOuterJoins
				(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString + 
					(
						UseQueryWhereFragment ? 
						( (IQueryable) persister).QueryWhereFragment(alias, true, true) :
						persister.WhereJoinFragment(alias, true, true) 
					)
				)	
				.SetOrderByClause(orderBy);


			classPersisters = new ILoadable[joins+1];
			lockModeArray = createLockModeArray(joins+1, LockMode.None);
			for (int i=0; i<joins; i++) classPersisters[i] = ((OuterJoinableAssociation)associations[i]).Subpersister;
			classPersisters[joins] = persister;
		}

		protected void RenderStatement(SqlString condition, ISessionFactoryImplementor factory) 
		{
			RenderStatement(condition, String.Empty, factory);
		}

		protected void RenderStatement(SqlString condition, string orderBy, ISessionFactoryImplementor factory) 
		{
			SqlSelectBuilder sqlBuilder = new SqlSelectBuilder(factory);

			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);
			sqlBuilder.SetSelectClause(
				(joins==0 ? String.Empty : SelectString(associations) + ",") +
				SelectString(persister, alias, suffixes[joins] )
				);

			sqlBuilder.SetFromClause(
				persister.FromTableFragment(alias) + 
				persister.FromJoinFragment(alias, true, true) 
				);

			sqlBuilder.AddWhereClause(condition);

			sqlBuilder.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString + 
				(
					UseQueryWhereFragment ? 
					( (IQueryable) persister).QueryWhereFragment(alias, true, true) :
					persister.WhereJoinFragment(alias, true, true) )
				);

			sqlBuilder.SetOrderByClause(orderBy);

			this.sqlString = sqlBuilder.ToSqlString();

			classPersisters = new ILoadable[joins+1];
			lockModeArray = createLockModeArray(joins+1, LockMode.None);
			for (int i=0; i<joins; i++) classPersisters[i] = ((OuterJoinableAssociation)associations[i]).Subpersister;
			classPersisters[joins] = persister;
		}

		/// <summary>
		/// Should we sue the discriminator, to narrow the select to
		/// instances of the queried subclass?
		/// </summary>
		/// <value>False, unless overridden</value>
		protected virtual bool UseQueryWhereFragment
		{
			get { return false;}
		}

	}
}
