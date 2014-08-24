namespace NHibernate.Cfg.MappingSchema
{
	public interface IEntityPropertyMapping: IDecoratable
	{
		string Name { get; }
		string Access { get; }
		bool OptimisticLock { get; }
		bool IsLazyProperty { get; }
	}
}