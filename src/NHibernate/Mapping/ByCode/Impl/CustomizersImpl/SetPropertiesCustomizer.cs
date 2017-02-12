using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class SetPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, ISetPropertiesMapper<TEntity, TElement>
	{
		public SetPropertiesCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, propertyPath, customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsSet(propertyPath.LocalMember);
		}
	}
}