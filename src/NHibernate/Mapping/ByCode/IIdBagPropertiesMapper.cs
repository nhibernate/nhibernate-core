using System;
namespace NHibernate.Mapping.ByCode
{
	public interface IIdBagPropertiesMapper : ICollectionPropertiesMapper
	{
		void Id(Action<ICollectionIdMapper> idMapping);
	}

	public interface IIdBagPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		void Id(Action<ICollectionIdMapper> idMapping);		
	}
}