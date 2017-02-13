using System;

namespace NHibernate.Mapping.ByCode
{
	public interface ISubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void DiscriminatorValue(object value);
		void Extends(System.Type baseType);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Abstract(bool isAbstract);
	}

	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
		void Join(string splitGroupId, Action<IJoinMapper> splitMapping);
	}

	public interface ISubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void DiscriminatorValue(object value);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Extends(System.Type baseType);
		void Abstract(bool isAbstract);
	}

	public interface ISubclassMapper<TEntity> : ISubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
		void Join(string splitGroupId, Action<IJoinMapper<TEntity>> splitMapping);
	}
}