using System;
using System.Text;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.Util;

namespace NHibernate.Loader {
	
	public class AbstractEntityLoader : OuterJoinLoader {
		protected ILoadable persister;
		protected string alias;

		public AbstractEntityLoader(ILoadable persister, ISessionFactoryImplementor factory) : base(factory.Dialect) {
			this.persister = persister;
			alias = Alias(persister.ClassName, 0);
		}

		protected void RenderStatement(string condition, ISessionFactoryImplementor factory) {
			RenderStatement(condition, StringHelper.EmptyString, factory);

		}

		protected void RenderStatement(string condition, string orderBy, ISessionFactoryImplementor factory)	{
			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) suffixes[i] = (joins==0) ? StringHelper.EmptyString : i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);
			sql = new Select()
				.SetSelectClause(
					(joins==0 ? StringHelper.EmptyString : SelectString(associations) + ",") +
					SelectString(persister, alias, suffixes[joins] )
				)
				.SetFromClause(
					persister.FromTableFragment(alias) + 
					persister.FromJoinFragment(alias, true, true)
				)
				.SetWhereClause(condition)
				.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString +
					persister.WhereJoinFragment(alias, true, true)
				)
				.SetOrderByClause(orderBy)
				.ToStatementString();

			classPersisters = new ILoadable[joins+1];
			for (int i=0; i<joins; i++) classPersisters[i] = ((OuterJoinableAssociation)associations[i]).Subpersister;
			classPersisters[joins] = persister;
		}
	}
}
