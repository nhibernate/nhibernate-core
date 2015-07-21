namespace NHibernate.Cfg.MappingSchema
{
	public interface IRelationship
	{
		string EntityName { get; }
		string Class { get; }
		HbmNotFoundMode NotFoundMode { get; }
	}
}