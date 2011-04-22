using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class DynamicComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IDynamicComponentMapper<TComponent> where TComponent : class
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
			throw new NotImplementedException();
		}

		public void Access(System.Type accessorType)
		{
			throw new NotImplementedException();
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			throw new NotImplementedException();
		}

		public void Update(bool consideredInUpdateQuery)
		{
			throw new NotImplementedException();
		}

		public void Insert(bool consideredInInsertQuery)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}