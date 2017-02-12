using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionPropertiesContainerMapper
	{
		void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping,
		         Action<ICollectionElementRelation> mapping);

		void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping,
		         Action<ICollectionElementRelation> mapping);

		void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping,
		          Action<ICollectionElementRelation> mapping);

		void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping,
		         Action<IMapKeyRelation> keyMapping,
		         Action<ICollectionElementRelation> mapping);

		void IdBag(MemberInfo property, Action<IIdBagPropertiesMapper> collectionMapping,
						 Action<ICollectionElementRelation> mapping);
	}

	public interface IPropertyContainerMapper : ICollectionPropertiesContainerMapper, IPlainPropertyContainerMapper {}

	public interface ICollectionPropertiesContainerMapper<TEntity>
	{
		void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                   Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
		                   Action<ICollectionElementRelation<TElement>> mapping);
		void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping);
		void Set<TElement>(string notVisiblePropertyOrFieldName,
											 Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void Set<TElement>(string notVisiblePropertyOrFieldName,
											 Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping);

		void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                   Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
		                   Action<ICollectionElementRelation<TElement>> mapping);
		void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping);
		void Bag<TElement>(string notVisiblePropertyOrFieldName,
											 Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void Bag<TElement>(string notVisiblePropertyOrFieldName,
											 Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping);

		void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                    Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
		                    Action<ICollectionElementRelation<TElement>> mapping);
		void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
												Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping);
		void List<TElement>(string notVisiblePropertyOrFieldName,
												Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
												Action<ICollectionElementRelation<TElement>> mapping);
		void List<TElement>(string notVisiblePropertyOrFieldName,
												Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping);

		void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
		                         Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
		                         Action<IMapKeyRelation<TKey>> keyMapping,
		                         Action<ICollectionElementRelation<TElement>> mapping);
		void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
		                         Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
		                         Action<ICollectionElementRelation<TElement>> mapping);
		void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
														 Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping);
		void Map<TKey, TElement>(string notVisiblePropertyOrFieldName,
												 Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
												 Action<IMapKeyRelation<TKey>> keyMapping,
												 Action<ICollectionElementRelation<TElement>> mapping);
		void Map<TKey, TElement>(string notVisiblePropertyOrFieldName,
														 Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
														 Action<ICollectionElementRelation<TElement>> mapping);
		void Map<TKey, TElement>(string notVisiblePropertyOrFieldName,
														 Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping);

		void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping);
		void IdBag<TElement>(string notVisiblePropertyOrFieldName,
											 Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void IdBag<TElement>(string notVisiblePropertyOrFieldName,
											 Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping);
	}

	public interface IPropertyContainerMapper<TEntity> : ICollectionPropertiesContainerMapper<TEntity>, IPlainPropertyContainerMapper<TEntity>
	{}
}