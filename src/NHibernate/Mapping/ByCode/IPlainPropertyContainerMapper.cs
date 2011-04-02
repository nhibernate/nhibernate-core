using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IBasePlainPropertyContainerMapper
	{
		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
		void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping);
	}

	public interface IPlainPropertyContainerMapper : IBasePlainPropertyContainerMapper
	{
		void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping);
	}

	public interface IPlainPropertyContainerMapper<TContainer>
	{
		void Property<TProperty>(Expression<Func<TContainer, TProperty>> property);
		void Property<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IPropertyMapper> mapping);
		void Property(FieldInfo member, Action<IPropertyMapper> mapping);

		void Component<TComponent>(Expression<Func<TContainer, TComponent>> property,
		                           Action<IComponentMapper<TComponent>> mapping) where TComponent : class;

		void ManyToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
		void ManyToOne<TProperty>(Expression<Func<TContainer, TProperty>> property) where TProperty : class;

		void OneToOne<TProperty>(Expression<Func<TContainer, TProperty>> property, Action<IOneToOneMapper> mapping) where TProperty : class;
		void Any<TProperty>(Expression<Func<TContainer, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class;
	}
}