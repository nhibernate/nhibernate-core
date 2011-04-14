using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class PropertyContainerCustomizer<TEntity> where TEntity : class
	{
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;

		public PropertyContainerCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.explicitDeclarationsHolder = explicitDeclarationsHolder;
			CustomizersHolder = customizersHolder;
			PropertyPath = propertyPath;
		}

		protected internal ICustomizersHolder CustomizersHolder { get; private set; }
		protected internal PropertyPath PropertyPath { get; private set; }

		protected internal IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder
		{
			get { return explicitDeclarationsHolder; }
		}

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			Property(property, x => { });
		}

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			RegisterPropertyMapping(property, mapping);
		}

		protected virtual void RegisterPropertyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
			explicitDeclarationsHolder.AddAsProperty(member);
			explicitDeclarationsHolder.AddAsProperty(memberOf);
		}

		public void Property(FieldInfo member, Action<IPropertyMapper> mapping)
		{
			RegisterFieldMapping(member, mapping);
		}

		protected virtual void RegisterFieldMapping(FieldInfo member, Action<IPropertyMapper> mapping)
		{
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			explicitDeclarationsHolder.AddAsProperty(member);
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property,
		                                  Action<IComponentMapper<TComponent>> mapping) where TComponent : class
		{
			RegisterComponentMapping(property, mapping);
		}

		protected virtual void RegisterComponentMapping<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
			where TComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentCustomizer<TComponent>(explicitDeclarationsHolder, CustomizersHolder, new PropertyPath(PropertyPath, member)));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentCustomizer<TComponent>(explicitDeclarationsHolder, CustomizersHolder, new PropertyPath(PropertyPath, memberOf)));
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			RegisterManyToOneMapping(property, mapping);
		}

		protected virtual void RegisterManyToOneMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
			explicitDeclarationsHolder.AddAsManyToOneRelation(member);
			explicitDeclarationsHolder.AddAsManyToOneRelation(memberOf);
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property) where TProperty : class
		{
			ManyToOne(property, x => { });
		}

		public void OneToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IOneToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
			explicitDeclarationsHolder.AddAsOneToOneRelation(member);
			explicitDeclarationsHolder.AddAsOneToOneRelation(memberOf);
		}

		public void Any<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			RegisterAnyMapping(property, idTypeOfMetaType, mapping);
		}

		protected virtual void RegisterAnyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), am => am.IdType(idTypeOfMetaType));
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), mapping);
			CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, memberOf), am => am.IdType(idTypeOfMetaType));
			explicitDeclarationsHolder.AddAsAny(member);
			explicitDeclarationsHolder.AddAsAny(memberOf);
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                          Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterSetMapping(property, collectionMapping, mapping);
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Set(property, collectionMapping, x => { });
		}

		protected virtual void RegisterSetMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));

			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
		}

		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                          Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterBagMapping(property, collectionMapping, mapping);
		}
		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Bag(property, collectionMapping, x => { });
		}

		protected virtual void RegisterBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));

			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, memberOf), CustomizersHolder));
		}

		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                           Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
		                           Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterListMapping(property, collectionMapping, mapping);
		}
		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															 Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			List(property, collectionMapping, x => { });
		}

		protected virtual void RegisterListMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));

			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
		                                Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
		                                Action<IMapKeyRelation<TKey>> keyMapping,
		                                Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterMapMapping(property, collectionMapping, keyMapping, mapping);
		}
		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping)
		{
			Map(property, collectionMapping, keyMapping => { }, x => { });
		}

		protected virtual void RegisterMapMapping<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			var memberPath = new PropertyPath(PropertyPath, member);
			collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(explicitDeclarationsHolder, memberPath, CustomizersHolder));
			keyMapping(new MapKeyRelationCustomizer<TKey>(explicitDeclarationsHolder, memberPath, CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, memberPath, CustomizersHolder));

			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			var memberOfPath = new PropertyPath(PropertyPath, memberOf);
			collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(explicitDeclarationsHolder, memberOfPath, CustomizersHolder));
			keyMapping(new MapKeyRelationCustomizer<TKey>(explicitDeclarationsHolder, memberOfPath, CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, memberOfPath, CustomizersHolder));
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
																		Action<ICollectionElementRelation<TElement>> mapping)
		{
			Map(property, collectionMapping, keyMapping => { }, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping,
													Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterIdBagMapping(property, collectionMapping, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			RegisterIdBagMapping(property, collectionMapping, x => { });
		}

		protected virtual void RegisterIdBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			collectionMapping(new IdBagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, member), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));

			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			collectionMapping(new IdBagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(null, memberOf), CustomizersHolder));
			mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, memberOf), CustomizersHolder));
		}
	}
}