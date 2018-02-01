using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate.Impl
{
	// 6.0 TODO: merge into 'ICriteria'.
	public interface ISupportEntityJoinCriteria
	{
		ICriteria CreateEntityCriteria(string alias, ICriterion withClause, JoinType joinType, string entityName);
	}
}
