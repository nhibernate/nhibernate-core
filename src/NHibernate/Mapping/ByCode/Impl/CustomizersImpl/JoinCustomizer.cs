using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class JoinCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IJoinMapper<TEntity>
		where TEntity : class
	{
		private readonly string splitGroupId;
		private readonly IKeyMapper<TEntity> keyMapper;

		public JoinCustomizer(string splitGroupId, IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.splitGroupId = splitGroupId;
			keyMapper = new JoinKeyCustomizer<TEntity>(customizersHolder);
		}

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.SqlDelete(sql));
		}

		public void Subselect(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Subselect(sql));
		}

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Schema(schemaName));
		}

		public void Key(Action<IKeyMapper<TEntity>> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void Inverse(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Inverse(value));
		}

		public void Optional(bool isOptional)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Optional(isOptional));
		}

		public void Fetch(FetchKind fetchMode)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinAttributesMapper m) => m.Fetch(fetchMode));
		}

		protected override void RegisterSetMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof (TEntity), splitGroupId, member));
			base.RegisterSetMapping(property, collectionMapping, mapping);
		}

		protected override void RegisterBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterBagMapping(property, collectionMapping, mapping);
		}

		protected override void RegisterListMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterListMapping(property, collectionMapping, mapping);
		}

		protected override void RegisterMapMapping<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterMapMapping(property, collectionMapping, keyMapping, mapping);
		}

		protected override void RegisterPropertyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterPropertyMapping(property, mapping);
		}

		protected override void RegisterNoVisiblePropertyMapping(string notVisiblePropertyOrFieldName, System.Action<IPropertyMapper> mapping)
		{
			MemberInfo member = typeof(TEntity).GetPropertyOrFieldMatchingName(notVisiblePropertyOrFieldName);

			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterNoVisiblePropertyMapping(notVisiblePropertyOrFieldName, mapping);
		}

		protected override void RegisterComponentMapping<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterComponentMapping(property, mapping);
		}

		protected override void RegisterManyToOneMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterManyToOneMapping(property, mapping);
		}

		protected override void RegisterAnyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterAnyMapping(property, idTypeOfMetaType, mapping);
		}

		protected override void RegisterIdBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.RegisterIdBagMapping(property, collectionMapping, mapping);
		}
	}
}