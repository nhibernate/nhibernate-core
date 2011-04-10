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

		public override void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof (TEntity), splitGroupId, member));
			base.Set(property, collectionMapping, mapping);
		}

		public override void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Bag(property, collectionMapping, mapping);
		}

		public override void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.List(property, collectionMapping, mapping);
		}

		public override void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Map(property, collectionMapping, keyMapping, mapping);
		}

		public override void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Map(property, collectionMapping, mapping);
		}

		public override void Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Property(property);
		}

		public override void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Property(property, mapping);
		}

		public override void Property(FieldInfo member, Action<IPropertyMapper> mapping)
		{
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Property(member, mapping);
		}

		public override void Component<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Component(property, mapping);
		}

		public override void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.ManyToOne(property, mapping);
		}

		public override void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.ManyToOne(property);
		}

		public override void Any<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.Any(property, idTypeOfMetaType, mapping);
		}

		public override void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			ExplicitDeclarationsHolder.AddAsPropertySplit(new SplitDefinition(typeof(TEntity), splitGroupId, member));
			base.IdBag(property, collectionMapping, mapping);
		}
	}
}