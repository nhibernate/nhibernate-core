namespace NHibernate.Mapping.ByCode
{
	public interface IMapPropertiesMapper : ICollectionPropertiesMapper {}

	public interface IMapPropertiesMapper<TEntity, TKey, TElement> : ICollectionPropertiesMapper<TEntity, TElement>
	{}
}