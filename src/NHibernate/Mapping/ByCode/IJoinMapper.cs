using System;

namespace NHibernate.Mapping.ByCode
{
	public interface IJoinAttributesMapper : IEntitySqlsMapper
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Key(Action<IKeyMapper> keyMapping);
		void Inverse(bool value);
		void Optional(bool isOptional);
		void Fetch(FetchKind fetchMode);
	}

	public interface IJoinMapper : IJoinAttributesMapper, ICollectionPropertiesContainerMapper, IBasePlainPropertyContainerMapper { }

	public interface IJoinAttributesMapper<TEntity> : IEntitySqlsMapper where TEntity : class
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Inverse(bool value);
		void Optional(bool isOptional);
		void Fetch(FetchKind fetchMode);
		void Key(Action<IKeyMapper<TEntity>> keyMapping);
	}

	public interface IJoinMapper<TEntity> : IJoinAttributesMapper<TEntity>, ICollectionPropertiesContainerMapper<TEntity>, IBasePlainPropertyContainerMapper<TEntity> 
		where TEntity : class {}
}