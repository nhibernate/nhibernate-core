using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class MapPropertiesCustomizer<TEntity, TKey, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IMapPropertiesMapper<TEntity, TKey, TElement>
		where TEntity : class
	{
		public MapPropertiesCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, propertyPath, customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsMap(propertyPath.LocalMember);
		}
	}
}