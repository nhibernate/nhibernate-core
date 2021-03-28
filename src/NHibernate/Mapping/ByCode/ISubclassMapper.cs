using System;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Util;

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
	
	public static class SubclassAttributesMapperExtensions
	{
		//6.0 TODO: Merge to ISubclassAttributesMapper<TEntity>
		public static void Extends<TEntity>(this ISubclassAttributesMapper<TEntity> mapper, string entityOrClassName)
			where TEntity : class
		{
			switch (mapper)
			{
				case SubclassCustomizer<TEntity> sc:
					sc.Extends(entityOrClassName);
					break;
				case PropertyContainerCustomizer<TEntity> pcc:
					pcc.CustomizersHolder.AddCustomizer(
						typeof(TEntity),
						(ISubclassMapper m) => m.Extends(entityOrClassName));
					break;
				default:
					throw new ArgumentException($@"{mapper.GetType()} requires to extend {typeof(SubclassCustomizer<TEntity>).FullName} or {typeof(PropertyContainerCustomizer<TEntity>).FullName} to support Extends(entityOrClassName).");
			}
		}

		//6.0 TODO: Merge to ISubclassAttributesMapper
		public static void Extends(this ISubclassAttributesMapper mapper, string entityOrClassName)
		{
			ReflectHelper.CastOrThrow<SubclassMapper>(mapper, "Extends(entityOrClassName)").Extends(entityOrClassName);
		}
	}
}
