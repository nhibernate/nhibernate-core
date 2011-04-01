using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ComponentElementCustomizer<TComponent> : IComponentElementMapper<TComponent>
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;
		private readonly PropertyPath propertyPath;

		public ComponentElementCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.explicitDeclarationsHolder = explicitDeclarationsHolder;
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IComponentElementMapper<TComponent> Members

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Parent(member));
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Parent(member, parentMapping));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Insert(consideredInInsertQuery));
		}

		public void Lazy(bool isLazy)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Lazy(isLazy));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Class(typeof (TConcrete)));
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, memberOf), mapping);
		}

		public void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property, Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(explicitDeclarationsHolder, new PropertyPath(propertyPath, member), customizersHolder));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(explicitDeclarationsHolder, new PropertyPath(propertyPath, memberOf), customizersHolder));
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, memberOf), mapping);
		}

		public void Access(Accessor accessor)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		#endregion
	}
}