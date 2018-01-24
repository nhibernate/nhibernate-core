using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate.Impl
{
	public interface ISupportEntityJoinCriteria
	{
		ICriteria CreateEntityCriteria(string alias, ICriterion withClause, JoinType joinType, string entityName);
	}
}
