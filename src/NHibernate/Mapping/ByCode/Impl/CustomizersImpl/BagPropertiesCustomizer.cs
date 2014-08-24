using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class BagPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IBagPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		public BagPropertiesCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, propertyPath, customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsBag(propertyPath.LocalMember);
		}
	}
}