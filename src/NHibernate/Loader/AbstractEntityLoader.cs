using System;
using System.Text;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader 
{
	
	public class AbstractEntityLoader : OuterJoinLoader 
	{
		private ILoadable persister;
		private string alias;

		public AbstractEntityLoader(ILoadable persister, ISessionFactoryImplementor factory) : base(factory.Dialect) 
		{
			this.persister = persister;
			alias = ToAlias(persister.ClassName, 0);
		}

		protected string Alias 
		{
			get { return alias; }
		}

		/// <summary>
		/// Gets the <see cref="ILoadable"/> Persister.
		/// </summary>
		protected ILoadable Persister 
		{
			get { return persister; }
		}

		protected void RenderStatement(SqlSelectBuilder selectBuilder, ISessionFactoryImplementor factory) 
		{
			RenderStatement(selectBuilder, String.Empty, factory);
		}

		protected void RenderStatement(SqlSelectBuilder selectBuilder, string orderBy, ISessionFactoryImplementor factory) 
		{
			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			Suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) Suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);

			selectBuilder.SetSelectClause(
				(joins==0 ? String.Empty : SelectString(associations) + ",") +
				SelectString(persister, alias, Suffixes[joins] )
				)
				.SetFromClause
				(
					persister.FromTableFragment(alias).Append(
						persister.FromJoinFragment(alias, true, true)
					)
				)
				.SetOuterJoins
				(			
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString.Append( 
						UseQueryWhereFragment ? 
						( (IQueryable) persister).QueryWhereFragment(alias, true, true) :
						persister.WhereJoinFragment(alias, true, true) 
					)
				)	
				.SetOrderByClause(orderBy);

			Persisters = new ILoadable[joins+1];
//			classPersisters = new ILoadable[joins+1];
			LockModeArray = CreateLockModeArray(joins+1, LockMode.None);
			for (int i=0; i<joins; i++) 
			{
				Persisters[i] = ((OuterJoinableAssociation)associations[i]).Subpersister;
			}
			Persisters[joins] = persister;
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
			Suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) Suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);
			sqlBuilder.SetSelectClause(
				(joins==0 ? String.Empty : SelectString(associations) + ",") +
				SelectString(persister, alias, Suffixes[joins] )
				);

			sqlBuilder.SetFromClause(
				persister.FromTableFragment(alias).Append(
					persister.FromJoinFragment(alias, true, true)
				)
			);

			sqlBuilder.AddWhereClause(condition);

			sqlBuilder.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString.Append 
				(
					UseQueryWhereFragment ? 
					( (IQueryable) persister).QueryWhereFragment(alias, true, true) :
					persister.WhereJoinFragment(alias, true, true) )
				);

			sqlBuilder.SetOrderByClause(orderBy);

			this.SqlString = sqlBuilder.ToSqlString();

			Persisters = new ILoadable[joins+1];
			LockModeArray = CreateLockModeArray(joins+1, LockMode.None);
			for (int i=0; i<joins; i++) 
			{
				Persisters[i] = ((OuterJoinableAssociation)associations[i]).Subpersister;
			}
			Persisters[joins] = persister;
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
