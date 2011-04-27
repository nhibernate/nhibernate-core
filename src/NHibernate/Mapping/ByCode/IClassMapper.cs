using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IClassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Id(Action<IIdMapper> idMapper);
		void Id(MemberInfo idProperty, Action<IIdMapper> idMapper);

		void ComponentAsId(MemberInfo idProperty, Action<IComponentAsIdMapper> idMapper);
		void ComposedId(Action<IComposedIdMapper> idPropertiesMapping);

		void Discriminator(Action<IDiscriminatorMapper> discriminatorMapping);
		void DiscriminatorValue(object value);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Mutable(bool isMutable);
		void Version(MemberInfo versionProperty, Action<IVersionMapper> versionMapping);
		void NaturalId(Action<INaturalIdMapper> naturalIdMapping);
		void Cache(Action<ICacheMapper> cacheMapping);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Where(string whereClause);
		void SchemaAction(SchemaAction action);
	}

	public interface IClassMapper : IClassAttributesMapper, IPropertyContainerMapper
	{
		/// <summary>
		/// Using the Join, it is possible to split properties of one class to several tables, when there's a 1-to-1 relationship between the table
		/// </summary>
		/// <param name="splitGroupId">The split-group identifier. By default it is assigned to the join-table-name</param>
		/// <param name="splittedMapping">The lambda to map the join.</param>
		void Join(string splitGroupId, Action<IJoinMapper> splittedMapping);
	}

	public interface IClassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty);
		void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper);
		void Id(FieldInfo idProperty, Action<IIdMapper> idMapper);

		void ComponentAsId<TComponent>(Expression<Func<TEntity, TComponent>> idProperty) where TComponent : class;
		void ComponentAsId<TComponent>(Expression<Func<TEntity, TComponent>> idProperty, Action<IComponentAsIdMapper<TComponent>> idMapper) where TComponent : class;

		void ComposedId(Action<IComposedIdMapper<TEntity>> idPropertiesMapping);

		void Discriminator(Action<IDiscriminatorMapper> discriminatorMapping);
		void DiscriminatorValue(object value);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Mutable(bool isMutable);
		void Version<TProperty>(Expression<Func<TEntity, TProperty>> versionProperty, Action<IVersionMapper> versionMapping);
		void NaturalId(Action<IBasePlainPropertyContainerMapper<TEntity>> naturalIdPropertiesMapping, Action<INaturalIdAttributesMapper> naturalIdMapping);
		void NaturalId(Action<IBasePlainPropertyContainerMapper<TEntity>> naturalIdPropertiesMapping);
		void Cache(Action<ICacheMapper> cacheMapping);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Where(string whereClause);
		void SchemaAction(SchemaAction action);
	}

	public interface IClassMapper<TEntity> : IClassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
		void Join(string splitGroupId, Action<IJoinMapper<TEntity>> splittedMapping);
	}
}