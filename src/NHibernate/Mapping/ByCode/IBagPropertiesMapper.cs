namespace NHibernate.Mapping.ByCode
{
	public interface IBagPropertiesMapper : ICollectionPropertiesMapper {}

	public interface IBagPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class {}
}