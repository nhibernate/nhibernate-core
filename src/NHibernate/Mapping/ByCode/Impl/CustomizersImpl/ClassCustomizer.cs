using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ClassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IClassMapper<TEntity>, IConformistHoldersProvider where TEntity : class
	{
		private Dictionary<string, IJoinMapper<TEntity>> joinCustomizers;

		public ClassCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsRootEntity(typeof (TEntity));

			// Add an empty customizer as a way to register the class as explicity declared
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => { });
		}

		private Dictionary<string, IJoinMapper<TEntity>> JoinCustomizers
		{
			get { return joinCustomizers ?? (joinCustomizers = new Dictionary<string, IJoinMapper<TEntity>>()); }
		}

		#region Implementation of IClassAttributesMapper<TEntity>

		public void OptimisticLock(OptimisticLockMode mode)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.OptimisticLock(mode));
		}

		public void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty)
		{
			Id(idProperty, x => { });
		}

		public void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper)
		{
			MemberInfo member = null;
			if (idProperty != null)
			{
				member = TypeExtensions.DecodeMemberAccessExpression(idProperty);
				ExplicitDeclarationsHolder.AddAsPoid(member);
			}
			CustomizersHolder.AddCustomizer(typeof (TEntity), m => m.Id(member, idMapper));
		}

		public void Id(string notVisiblePropertyOrFieldName, Action<IIdMapper> idMapper)
		{
			MemberInfo member = null;
			if (notVisiblePropertyOrFieldName != null)
			{
				member = typeof(TEntity).GetPropertyOrFieldMatchingName(notVisiblePropertyOrFieldName);
				ExplicitDeclarationsHolder.AddAsPoid(member);
			}
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Id(member, idMapper));
		}

		public void ComponentAsId<TComponent>(Expression<Func<TEntity, TComponent>> idProperty) where TComponent : class
		{
			ComponentAsId(idProperty, x => { });
		}

		public void ComponentAsId<TComponent>(Expression<Func<TEntity, TComponent>> idProperty, Action<IComponentAsIdMapper<TComponent>> idMapper) where TComponent : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(idProperty);
			var propertyPath = new PropertyPath(null, member);
			idMapper(new ComponentAsIdCustomizer<TComponent>(ExplicitDeclarationsHolder, CustomizersHolder, propertyPath));
		}

		public void ComponentAsId<TComponent>(string notVisiblePropertyOrFieldName) where TComponent : class
		{
			ComponentAsId<TComponent>(notVisiblePropertyOrFieldName, x => { });
		}

		public void ComponentAsId<TComponent>(string notVisiblePropertyOrFieldName, Action<IComponentAsIdMapper<TComponent>> idMapper) where TComponent : class
		{
			var member = typeof(TEntity).GetPropertyOrFieldMatchingName(notVisiblePropertyOrFieldName);
			var propertyPath = new PropertyPath(null, member);
			idMapper(new ComponentAsIdCustomizer<TComponent>(ExplicitDeclarationsHolder, CustomizersHolder, propertyPath));
		}

		public void ComposedId(Action<IComposedIdMapper<TEntity>> idPropertiesMapping)
		{
			idPropertiesMapping(new ComposedIdCustomizer<TEntity>(ExplicitDeclarationsHolder, CustomizersHolder));
		}

		public void Discriminator(Action<IDiscriminatorMapper> discriminatorMapping)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IClassMapper m) => m.Discriminator(discriminatorMapping));
		}

		public void DiscriminatorValue(object value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.DiscriminatorValue(value));
		}

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Table(tableName));
		}

		public void Check(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Check(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Schema(schemaName));
		}

		public void Polymorphism(PolymorphismType type)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Polymorphism(type));
		}

		#endregion

		#region Implementation of IEntityAttributesMapper

		public void Mutable(bool isMutable)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Mutable(isMutable));
		}

		public void Version<TProperty>(Expression<Func<TEntity, TProperty>> versionProperty, Action<IVersionMapper> versionMapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(versionProperty);
			ExplicitDeclarationsHolder.AddAsVersionProperty(member);
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Version(member, versionMapping));
		}

		public void Version(string notVisiblePropertyOrFieldName, Action<IVersionMapper> versionMapping)
		{
			var member = typeof(TEntity).GetPropertyOrFieldMatchingName(notVisiblePropertyOrFieldName);
			ExplicitDeclarationsHolder.AddAsVersionProperty(member);
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Version(member, versionMapping));
		}

		public void NaturalId(Action<IBasePlainPropertyContainerMapper<TEntity>> naturalIdPropertiesMapping, Action<INaturalIdAttributesMapper> naturalIdMapping)
		{
			naturalIdPropertiesMapping(new NaturalIdCustomizer<TEntity>(ExplicitDeclarationsHolder, CustomizersHolder));
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.NaturalId(nidm => naturalIdMapping(nidm)));
		}

		public void NaturalId(Action<IBasePlainPropertyContainerMapper<TEntity>> naturalIdPropertiesMapping)
		{
			NaturalId(naturalIdPropertiesMapping, mapAttr => { });
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Cache(cacheMapping));
		}

		public void Filter(string filterName, Action<IFilterMapper> filterMapping)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Filter(filterName, filterMapping));
		}

		public void Where(string whereClause)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Where(whereClause));
		}

		public void SchemaAction(SchemaAction action)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.SchemaAction(action));
		}

		public void Join(string splitGroupId, Action<IJoinMapper<TEntity>> splitMapping)
		{
			// add the customizer only to create the JoinMapper instance
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Join(splitGroupId, j => { }));

			IJoinMapper<TEntity> joinCustomizer;
			if (!JoinCustomizers.TryGetValue(splitGroupId, out joinCustomizer))
			{
				joinCustomizer = new JoinCustomizer<TEntity>(splitGroupId, ExplicitDeclarationsHolder, CustomizersHolder);
				JoinCustomizers.Add(splitGroupId, joinCustomizer);
			}
			splitMapping(joinCustomizer);
		}

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.EntityName(value));
		}

		public void Proxy(System.Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Persister<T>());
		}

		public void Synchronize(params string[] table)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Synchronize(table));
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.SqlDelete(sql));
		}

		public void Subselect(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassMapper m) => m.Subselect(sql));
		}

		#endregion

		ICustomizersHolder IConformistHoldersProvider.CustomizersHolder
		{
			get { return CustomizersHolder; }
		}

		IModelExplicitDeclarationsHolder IConformistHoldersProvider.ExplicitDeclarationsHolder
		{
			get { return ExplicitDeclarationsHolder; }
		}
	}
}