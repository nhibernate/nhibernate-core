using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class DynamicComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IDynamicComponentMapper<TComponent>
	{
		public DynamicComponentCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsDynamicComponent(propertyPath.LocalMember, typeof(TComponent));
		}

		#region IDynamicComponentMapper<TComponent> Members

		public void Access(Accessor accessor)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Insert(consideredInInsertQuery));
		}

		public void Unique(bool unique)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IDynamicComponentAttributesMapper m) => m.Unique(unique));
		}

		#endregion
	}
}