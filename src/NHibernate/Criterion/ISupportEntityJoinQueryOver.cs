using System;
using System.Linq.Expressions;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	//TODO: Make interface more flexible for changes (maybe it should take only 2 params alias + EntityJoinConfig)
	public interface ISupportEntityJoinQueryOver
	{
		void JoinEntityAlias<U>(Expression<Func<U>> alias, JoinType joinType, ICriterion withClause, string entityName);
	}
	
	public interface ISupportEntityJoinQueryOver<TRoot>: ISupportEntityJoinQueryOver
	{
		IQueryOver<TRoot, U> JoinEntityQueryOver<U>(Expression<Func<U>> alias, JoinType joinType, ICriterion withClause, string entityName);
	}
}
