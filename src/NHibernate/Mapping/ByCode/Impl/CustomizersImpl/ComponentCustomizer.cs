using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IComponentMapper<TComponent>, IConformistHoldersProvider
	{
		public ComponentCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsComponent(typeof (TComponent));
		}

		public ComponentCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
			: base(explicitDeclarationsHolder, customizersHolder, propertyPath)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsComponent(typeof (TComponent));
			if(propertyPath != null)
			{
				explicitDeclarationsHolder.AddAsPersistentMember(propertyPath.LocalMember);
			}
		}

		#region Implementation of IComponentMapper<TComponent>

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			Parent(parent, x => { });
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			AddCustomizer(m => m.Parent(member, parentMapping));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			AddCustomizer(m => m.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			AddCustomizer(m => m.Insert(consideredInInsertQuery));
		}

		public void Lazy(bool isLazy)
		{
			AddCustomizer(m => m.Lazy(isLazy));
		}

		public void Unique(bool unique)
		{
			AddCustomizer(m=>m.Unique(unique));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			AddCustomizer(m => m.Class(typeof (TConcrete)));
		}

		#endregion

		#region IComponentMapper<TComponent> Members

		public void Access(Accessor accessor)
		{
			AddCustomizer(m => m.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			AddCustomizer(m => m.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			AddCustomizer(m => m.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		#endregion

		private void AddCustomizer(Action<IComponentAttributesMapper> classCustomizer)
		{
			if (PropertyPath == null)
			{
				CustomizersHolder.AddCustomizer(typeof (TComponent), classCustomizer);
			}
			else
			{
				CustomizersHolder.AddCustomizer(PropertyPath, classCustomizer);
			}
		}

		#region IConformistHoldersProvider Members

		ICustomizersHolder IConformistHoldersProvider.CustomizersHolder
		{
			get { return CustomizersHolder; }
		}

		IModelExplicitDeclarationsHolder IConformistHoldersProvider.ExplicitDeclarationsHolder
		{
			get { return ExplicitDeclarationsHolder; }
		}

		#endregion
	}
}