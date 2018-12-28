using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode
{
	public interface IComponentAttributesMapper : IEntityPropertyMapper
	{
		void Parent(MemberInfo parent);
		void Parent(MemberInfo parent, Action<IComponentParentMapper> parentMapping);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
		void Unique(bool unique);
		void Class(System.Type componentType);
	}

	public interface IComponentMapper : IComponentAttributesMapper, IPropertyContainerMapper {}

	public interface IComponentAttributesMapper<TComponent> : IEntityPropertyMapper
	{
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class;
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class;
		void Parent(string notVisiblePropertyOrFieldName, Action<IComponentParentMapper> mapping);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
		void Unique(bool unique);
		void Class<TConcrete>() where TConcrete : TComponent;
	}

	public interface IComponentMapper<TComponent> : IComponentAttributesMapper<TComponent>, IPropertyContainerMapper<TComponent>
	{}

	public static class ComponentAttributesMapper
	{
		// 6.0 TODO: Move to IComponentAttributesMapper
		public static void LazyGroup(this IComponentAttributesMapper mapper, string name)
		{
			if (mapper is ComponentMapper component)
			{
				component.LazyGroup(name);
			}
		}

		// 6.0 TODO: Move to IComponentAttributesMapper<TComponent>
		public static void LazyGroup<TComponent>(this IComponentAttributesMapper<TComponent> mapper, string name)
		{
			if (mapper is ComponentCustomizer<TComponent> component)
			{
				component.LazyGroup(name);
			}
			else if (mapper is ComponentElementCustomizer<TComponent> componentElement)
			{
				componentElement.LazyGroup(name);
			}
		}
	}
}
