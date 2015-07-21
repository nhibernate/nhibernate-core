namespace NHibernate.Cfg.MappingSchema
{
	public interface IEntityDiscriminableMapping
	{
		string DiscriminatorValue { get; }
	}
}