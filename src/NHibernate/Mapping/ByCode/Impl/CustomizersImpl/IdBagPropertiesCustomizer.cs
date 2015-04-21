using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class IdBagPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IIdBagPropertiesMapper<TEntity, TElement>
	{
		public IdBagPropertiesCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, propertyPath, customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsIdBag(propertyPath.LocalMember);
		}

		public void Id(Action<ICollectionIdMapper> idMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IIdBagPropertiesMapper x) => x.Id(idMapping));
		}
	}
}