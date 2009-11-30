namespace NHibernate.Cfg.MappingSchema
{
	public interface IEntityPropertyMapping: IDecoratable
	{
		string Name { get; }
		string Access { get; }
		bool OptimisticKock { get; }
	}
}