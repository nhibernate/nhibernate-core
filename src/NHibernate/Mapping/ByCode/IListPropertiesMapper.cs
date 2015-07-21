using System;

namespace NHibernate.Mapping.ByCode
{
	public interface IListPropertiesMapper : ICollectionPropertiesMapper
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}

	public interface IListPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement>
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}
}