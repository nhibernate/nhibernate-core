using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public interface IMinimalPlainPropertyContainerMapper
	{
		void Property(MemberInfo property, Action<IPropertyMapper> mapping);
		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
	}

	public interface IBasePlainPropertyContainerMapper : IMinimalPlainPropertyContainerMapper
	{
		void Component(MemberInfo property, Action<IComponentMapper> mapping);
		void Component(MemberInfo property, Action<IDynamicComponentMapper> mapping);

		void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping);
	}

	public interface IPlainPropertyContainerMapper : IBasePlainPropertyContainerMapper
	{
		void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping);
	}

	public interface IMinimalPlainPropertyContainerMapper<TContainer>
	{
		void Property<TProperty>(Expression<Func<TContainer, TProperty>> property);
		void Property<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IPropertyMapper> mapping);
		void Property(string notVisiblePropertyOrFieldName, Action<IPropertyMapper> mapping);

		void ManyToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
		void ManyToOne<TProperty>(Expression<Func<TContainer, TProperty>> property) where TProperty : class;
		void ManyToOne<TProperty>(string notVisiblePropertyOrFieldName, Action<IManyToOneMapper> mapping) where TProperty : class;
	}

	public interface IBasePlainPropertyContainerMapper<TContainer> : IMinimalPlainPropertyContainerMapper<TContainer>
	{
		void Component<TComponent>(Expression<Func<TContainer, TComponent>> property, Action<IComponentMapper<TComponent>> mapping);
		void Component<TComponent>(Expression<Func<TContainer, TComponent>> property);
		void Component<TComponent>(Expression<Func<TContainer, IDictionary>> property, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping);

		void Component<TComponent>(string notVisiblePropertyOrFieldName, Action<IComponentMapper<TComponent>> mapping);
		void Component<TComponent>(string notVisiblePropertyOrFieldName);
		void Component<TComponent>(string notVisiblePropertyOrFieldName, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping);

		void Any<TProperty>(Expression<Func<TContainer, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class;
		void Any<TProperty>(string notVisiblePropertyOrFieldName, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class;
	}

	public interface IPlainPropertyContainerMapper<TContainer> : IBasePlainPropertyContainerMapper<TContainer>
	{
		void OneToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IOneToOneMapper<TProperty>> mapping) where TProperty : class;
		void OneToOne<TProperty>(string notVisiblePropertyOrFieldName, Action<IOneToOneMapper<TProperty>> mapping) where TProperty : class;
	}
	
	public static class BasePlainPropertyContainerMapperExtensions
	{
		//6.0 TODO: Merge into IBasePlainPropertyContainerMapper<> interface
		public static void Component<TContainer, TComponent>(
			this IBasePlainPropertyContainerMapper<TContainer> mapper,
			Expression<Func<TContainer, IDictionary<string, object>>> property,
			TComponent dynamicComponentTemplate,
			Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			var customizer = ReflectHelper.CastOrThrow<PropertyContainerCustomizer<TContainer>>(
				mapper, "mapping a generic <string, object> dictionary as a dynamic component");
			customizer.Component(property, dynamicComponentTemplate, mapping);
		}
	}
}
