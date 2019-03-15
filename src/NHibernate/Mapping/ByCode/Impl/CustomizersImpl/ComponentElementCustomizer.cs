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

		public void Parent(string notVisiblePropertyOrFieldName, Action<IComponentParentMapper> parentMapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVisiblePropertyOrFieldName);
			_customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Parent(member, parentMapping));
			_explicitDeclarationsHolder.AddAsPersistentMember(member);
		}

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

		public void LazyGroup(string name)
		{
			_customizersHolder.AddCustomizer(typeof(TComponent), x => x.LazyGroup(name));
		}

		public void Unique(bool unique)
		{
			_customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Unique(unique));
		}

		public void Class<TConcrete>() where TConcrete : TComponent
		{
			_customizersHolder.AddCustomizer(typeof (TComponent), (IComponentAttributesMapper x) => x.Class(typeof (TConcrete)));
		}

		public void Property(string notVisiblePropertyOrFieldName, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVisiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TComponent));
			_customizersHolder.AddCustomizer(new PropertyPath(_propertyPath, member), mapping);
			_explicitDeclarationsHolder.AddAsProperty(memberOf);
		}

		public static MemberInfo GetPropertyOrFieldMatchingNameOrThrow(string memberName)
		{
			var result = typeof(TComponent).GetPropertyOrFieldMatchingName(memberName);
			if (result == null)
			{
				throw new MappingException(string.Format("Member not found. The member '{0}' does not exists in type {1}", memberName, typeof(TComponent).FullName));
			}
			return result;
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

		public void Component<TNestedComponent>(string notVisiblePropertyOrFieldName, Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVisiblePropertyOrFieldName);
			mapping(new ComponentElementCustomizer<TNestedComponent>(_explicitDeclarationsHolder, new PropertyPath(_propertyPath, member), _customizersHolder));
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TComponent));
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

		public void ManyToOne<TProperty>(string notVisiblePropertyOrFieldName, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a many-to-one with {2} but was {3}",
					notVisiblePropertyOrFieldName, typeof(TComponent).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TComponent));
			_explicitDeclarationsHolder.AddAsManyToOneRelation(member);
			_explicitDeclarationsHolder.AddAsManyToOneRelation(memberOf);
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
