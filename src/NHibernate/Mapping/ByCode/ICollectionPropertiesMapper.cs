using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Mapping.ByCode
{
	public interface ICollectionPropertiesMapper : IEntityPropertyMapper, ICollectionSqlsMapper
	{
		void Inverse(bool value);
		void Mutable(bool value);
		void Where(string sqlWhereClause);
		void BatchSize(int value);
		void Lazy(CollectionLazy collectionLazy);
		void Key(Action<IKeyMapper> keyMapping);
		void OrderBy(MemberInfo property);
		void OrderBy(string sqlOrderByClause);
		void Sort();
		void Sort<TComparer>();
		void Cascade(Cascade cascadeStyle);
		//void Type(string namedCollectionType); // TODO: figure out a way to avoid string for embedded namedCollectionType
		void Type<TCollection>() where TCollection : IUserCollectionType;
		void Type(System.Type collectionType);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Cache(Action<ICacheMapper> cacheMapping);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Fetch(CollectionFetchMode fetchMode);
		void Persister(System.Type persister);
	}

	public interface ICollectionPropertiesMapper<TEntity, TElement> : IEntityPropertyMapper, ICollectionSqlsMapper
	{
		void Inverse(bool value);
		void Mutable(bool value);
		void Where(string sqlWhereClause);
		void BatchSize(int value);
		void Lazy(CollectionLazy collectionLazy);
		void Key(Action<IKeyMapper<TEntity>> keyMapping);
		void OrderBy<TProperty>(Expression<Func<TElement, TProperty>> property);
		void OrderBy(string sqlOrderByClause);
		void Sort();
		void Sort<TComparer>();
		void Cascade(Cascade cascadeStyle);
		void Type<TCollection>() where TCollection : IUserCollectionType;
		void Type(System.Type collectionType);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Cache(Action<ICacheMapper> cacheMapping);
		void Filter(string filterName, Action<IFilterMapper> filterMapping);
		void Fetch(CollectionFetchMode fetchMode);
		void Persister<TPersister>() where TPersister : ICollectionPersister;
	}
}