using System;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class DynamicComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>,
		IDynamicComponentMapper<TComponent>
	{
		private readonly System.Type _componentType;

		public DynamicComponentCustomizer(
			IModelExplicitDeclarationsHolder explicitDeclarationsHolder,
			ICustomizersHolder customizersHolder,
			PropertyPath propertyPath)
			: this(typeof(TComponent), explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
		}

		internal DynamicComponentCustomizer(
			System.Type componentType,
			IModelExplicitDeclarationsHolder explicitDeclarationsHolder,
			ICustomizersHolder customizersHolder,
			PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (propertyPath == null)
			{
				throw new ArgumentNullException(nameof(propertyPath));
			}

			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException(nameof(explicitDeclarationsHolder));
			}

			_componentType = componentType;
			explicitDeclarationsHolder.AddAsDynamicComponent(propertyPath.LocalMember, _componentType);
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

		protected override MemberInfo GetRequiredPropertyOrFieldByName(string memberName)
		{
			var result = _componentType.GetPropertyOrFieldMatchingName(memberName);
			if (result == null)
			{
				throw new MappingException(string.Format("Member not found. The member '{0}' does not exists in type {1}", memberName, _componentType.FullName));
			}
			return result;
		}
	}
}
