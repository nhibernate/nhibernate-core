using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IComponentElementMapper : IComponentAttributesMapper, IMinimalPlainPropertyContainerMapper
	{
		void Component(MemberInfo property, Action<IComponentElementMapper> mapping);
	}

	public interface IComponentElementMapper<TComponent> : IComponentAttributesMapper<TComponent>, IMinimalPlainPropertyContainerMapper<TComponent>
	{
		void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property,
										 Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class;

		void Component<TNestedComponent>(string notVisiblePropertyOrFieldName,
			Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class;
	}
}
