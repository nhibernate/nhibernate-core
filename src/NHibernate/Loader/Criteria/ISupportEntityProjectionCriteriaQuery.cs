using NHibernate.Criterion;

namespace NHibernate.Loader.Criteria
{
	// 6.0 TODO: merge into 'ICriteriaQuery'.
	public interface ISupportEntityProjectionCriteriaQuery
	{
		void RegisterEntityProjection(EntityProjection projection);

		ICriteria RootCriteria { get; }
	}
}
