namespace NHibernate.Mapping.ByCode
{
	public interface ISetPropertiesMapper : ICollectionPropertiesMapper {}

	public interface ISetPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class {}
}