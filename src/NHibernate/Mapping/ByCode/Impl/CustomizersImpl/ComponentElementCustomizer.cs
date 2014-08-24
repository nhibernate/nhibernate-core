using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ComponentElementCustomizer<TComponent> : IComponentElementMapper<TComponent>
	{
		private readonly ICustomizersHolder _customizersHolder;
		private readonly IModelExplicitDeclarationsHolder _explicitDeclarationsHolder;
		private readonly PropertyPath _propertyPath;

		public ComponentElementCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}

			_explicitDeclarationsHolder = explicitDeclarationsHolder;
			_propertyPath = propertyPath;
			_customizersHolder = customizersHolder;

			_explicitDeclarationsHolder.AddAsComponent(typeof(TComponent));
			
			if (propertyPath != null)
			{
				_explicitDeclarationsHolder.AddAsPersistentMember(propertyPath.LocalMember);
			}
		}

		#region IComponentElementMapper<TComponent> Members

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			Parent(parent, x => { });
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Parent(member, parentMapping));
			_explicitDeclarationsHolder.AddAsPersistentMember(member);
		}

		public void Update(bool consideredInUpdateQuery)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Update(consideredInUpdateQuery));
		}

		public void Insert(bool consideredInInsertQuery)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Insert(consideredInInsertQuery));
		}

		public void Lazy(bool isLazy)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Lazy(isLazy));
		}

		public void Unique(bool unique)
		{
			_customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Unique(unique));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Class(typeof (TConcrete)));
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			_customizersHolder.AddCustomizer(new PropertyPath(_propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			_customizersHolder.AddCustomizer(new PropertyPath(_propertyPath, memberOf), mapping);
			_explicitDeclarationsHolder.AddAsProperty(member);
			_explicitDeclarationsHolder.AddAsProperty(memberOf);
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property)
		{
			Property(property, x => { });
		}

		public void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property, Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(_explicitDeclarationsHolder, new PropertyPath(_propertyPath, member), _customizersHolder));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(_explicitDeclarationsHolder, new PropertyPath(_propertyPath, memberOf), _customizersHolder));
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			_customizersHolder.AddCustomizer(new PropertyPath(_propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			_customizersHolder.AddCustomizer(new PropertyPath(_propertyPath, memberOf), mapping);
			_explicitDeclarationsHolder.AddAsManyToOneRelation(member);
			_explicitDeclarationsHolder.AddAsManyToOneRelation(memberOf);
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property) where TProperty : class
		{
			ManyToOne(property, x => { });
		}

		public void Access(Accessor accessor)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Access(accessor));
		}

		public void Access(System.Type accessorType)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Access(accessorType));
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.OptimisticLock(takeInConsiderationForOptimisticLock));
		}

		#endregion
	}
}