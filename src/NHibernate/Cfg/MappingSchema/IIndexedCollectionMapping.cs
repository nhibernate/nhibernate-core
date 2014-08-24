namespace NHibernate.Cfg.MappingSchema
{
	public interface IIndexedCollectionMapping
	{
		HbmListIndex ListIndex { get; }
		HbmIndex Index { get; }
	}
}