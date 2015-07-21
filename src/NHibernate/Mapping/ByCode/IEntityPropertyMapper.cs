namespace NHibernate.Mapping.ByCode
{
	public interface IEntityPropertyMapper : IAccessorPropertyMapper
	{
		void OptimisticLock(bool takeInConsiderationForOptimisticLock);
	}
}