using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ListPropertiesCustomizer<TEntity, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IListPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		public ListPropertiesCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, propertyPath, customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsList(propertyPath.LocalMember);
		}

		#region Implementation of IListPropertiesMapper<TEntity,TElement>

		public void Index(Action<IListIndexMapper> listIndexMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IListPropertiesMapper x) => x.Index(listIndexMapping));
		}

		#endregion
	}
}