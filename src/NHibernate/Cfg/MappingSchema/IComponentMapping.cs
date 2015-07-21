namespace NHibernate.Cfg.MappingSchema
{
	public interface IComponentMapping : IPropertiesContainerMapping
	{
		string Class { get; }
		HbmParent Parent { get; }
		string EmbeddedNode { get; }
		string Name { get; }
	}
}