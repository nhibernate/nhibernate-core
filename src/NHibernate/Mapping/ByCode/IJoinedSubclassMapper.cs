using System;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public interface IJoinedSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Key(Action<IKeyMapper> keyMapping);
		void Extends(System.Type baseType);
		void SchemaAction(SchemaAction action);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Abstract(bool isAbstract);
	}

	public interface IJoinedSubclassMapper : IJoinedSubclassAttributesMapper, IPropertyContainerMapper {}

	public interface IJoinedSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Abstract(bool isAbstract);
		void Extends(System.Type baseType);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Key(Action<IKeyMapper<TEntity>> keyMapping);
		void SchemaAction(SchemaAction action);
        void Filter(string filterName, Action<IFilterMapper> filterMapping);
	}

	public interface IJoinedSubclassMapper<TEntity> : IJoinedSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class {}

	public static class JoinedSubclassAttributesMapperExtensions
	{
		//6.0 TODO: Merge to IJoinedSubclassAttributesMapper<TEntity>
		public static void Extends<TEntity>(this IJoinedSubclassAttributesMapper<TEntity> mapper, string entityOrClassName)
			where TEntity : class
		{
			switch (mapper)
			{
				case JoinedSubclassCustomizer<TEntity> jsc:
					jsc.Extends(entityOrClassName);
					break;
				case PropertyContainerCustomizer<TEntity> pcc:
					pcc.CustomizersHolder.AddCustomizer(
						typeof(TEntity),
						(IJoinedSubclassAttributesMapper m) => m.Extends(entityOrClassName));
					break;
				default:
					throw new ArgumentException($@"{mapper.GetType()} requires to extend {typeof(JoinedSubclassCustomizer<TEntity>).FullName} or {typeof(PropertyContainerCustomizer<TEntity>).FullName} to support Extends(entityOrClassName).");
			}
		}

		//6.0 TODO: Merge to IJoinedSubclassAttributesMapper
		public static void Extends(this IJoinedSubclassAttributesMapper mapper, string entityOrClassName)
		{
			ReflectHelper.CastOrThrow<JoinedSubclassMapper>(mapper, "Extends(entityOrClassName)").Extends(entityOrClassName);
		}
	}
}
