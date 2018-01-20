using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate.Impl
{
	public interface ISupportEntityJoinCriteria
	{
		//TODO: Make interface more flexible for changes (maybe it should be as simple as CreateEntityCriteria(EntityJoinConfig join)
		ICriteria CreateEntityCriteria(string alias, JoinType joinType, ICriterion withClause, string entityName);
	}
}
