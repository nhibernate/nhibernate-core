using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ComponentAsIdCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IComponentAsIdMapper<TComponent> where TComponent : class
	{
		public ComponentAsIdCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			if (propertyPath == null)
			{
				throw new ArgumentNullException("propertyPath");
			}
			explicitDeclarationsHolder.AddAsComponent(typeof (TComponent));
			explicitDeclarationsHolder.AddAsPoid(propertyPath.LocalMember);
		}

		public void UnsavedValue(UnsavedValueType unsavedValueType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IComponentAsIdAttributesMapper m) => m.UnsavedValue(unsavedValueType));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IComponentAsIdAttributesMapper m) => m.Class(typeof(TConcrete)));
		}

		public void Access(Accessor accessor)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IComponentAsIdAttributesMapper m) => m.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IComponentAsIdAttributesMapper m) => m.Access(accessorType));
		}
	}
}