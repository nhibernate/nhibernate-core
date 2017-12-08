using NHibernate.Criterion;

namespace NHibernate.Loader.Criteria
{
	public interface ISupportEntityProjectionCriteriaQuery
	{
		void RegisterEntityProjection(EntityProjection projection);
	}
}