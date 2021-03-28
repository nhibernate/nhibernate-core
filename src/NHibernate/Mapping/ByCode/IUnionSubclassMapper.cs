using System;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public interface IUnionSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Extends(System.Type baseType);
		void Abstract(bool isAbstract);
	}

	public interface IUnionSubclassMapper : IUnionSubclassAttributesMapper, IPropertyContainerMapper {}

	public interface IUnionSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Extends(System.Type baseType);
		void Abstract(bool isAbstract);
	}

	public interface IUnionSubclassMapper<TEntity> : IUnionSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class {}

	public static class UnionSubclassAttributesMapperExtensions
	{
		//6.0 TODO: Merge to IUnionSubclassAttributesMapper<TEntity>
		public static void Extends<TEntity>(this IUnionSubclassAttributesMapper<TEntity> mapper, string entityOrClassName)
			where TEntity : class
		{
			switch (mapper)
			{
				case UnionSubclassCustomizer<TEntity> usc:
					usc.Extends(entityOrClassName);
					break;
				case PropertyContainerCustomizer<TEntity> pcc:
					pcc.CustomizersHolder.AddCustomizer(
						typeof(TEntity),
						(IUnionSubclassAttributesMapper m) => m.Extends(entityOrClassName));
					break;
				default:
					throw new ArgumentException($@"{mapper.GetType()} requires to extend {typeof(UnionSubclassCustomizer<TEntity>).FullName} or {typeof(PropertyContainerCustomizer<TEntity>).FullName} to support Extends(entityOrClassName).");
			}
		}

		//6.0 TODO: Merge to IUnionSubclassAttributesMapper
		public static void Extends(this IUnionSubclassAttributesMapper mapper, string entityOrClassName)
		{
			ReflectHelper.CastOrThrow<UnionSubclassMapper>(mapper, "Extends(entityOrClassName)").Extends(entityOrClassName);
		}
	}
}
