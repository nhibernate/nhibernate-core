using System;
using System.Linq.Expressions;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	// 6.0 TODO: merge into IQueryOver.
	public interface ISupportEntityJoinQueryOver
	{
		void JoinEntityAlias<U>(Expression<Func<U>> alias, ICriterion withClause, JoinType joinType, string entityName);
	}

	// 6.0 TODO: merge into IQueryOver<TRoot,TSubType>.
	public interface ISupportEntityJoinQueryOver<TRoot>: ISupportEntityJoinQueryOver
	{
		IQueryOver<TRoot, U> JoinEntityQueryOver<U>(Expression<Func<U>> alias, ICriterion withClause, JoinType joinType, string entityName);
	}
}
