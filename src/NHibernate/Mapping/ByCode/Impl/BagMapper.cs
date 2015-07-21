using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class BagMapper : IBagPropertiesMapper
	{
		private readonly IAccessorPropertyMapper entityPropertyMapper;
		private readonly KeyMapper keyMapper;
		private readonly HbmBag mapping;
		private ICacheMapper cacheMapper;

		public BagMapper(System.Type ownerType, System.Type elementType, HbmBag mapping)
			: this(ownerType, elementType, new AccessorPropertyMapper(ownerType, mapping.Name, x => mapping.access = x), mapping) {}

		public BagMapper(System.Type ownerType, System.Type elementType, IAccessorPropertyMapper accessorMapper, HbmBag mapping)
		{
			if (ownerType == null)
			{
				throw new ArgumentNullException("ownerType");
			}
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			OwnerType = ownerType;
			ElementType = elementType;
			this.mapping = mapping;
			if (mapping.Key == null)
			{
				mapping.key = new HbmKey();
			}
			keyMapper = new KeyMapper(ownerType, mapping.Key);
			entityPropertyMapper = accessorMapper;
		}

		public System.Type OwnerType { get; private set; }
		public System.Type ElementType { get; private set; }

		#region Implementation of ICollectionPropertiesMapper

		public void Inverse(bool value)
		{
			mapping.inverse = value;
		}

		public void Mutable(bool value)
		{
			mapping.mutable = value;
		}

		public void Where(string sqlWhereClause)
		{
			mapping.where = sqlWhereClause;
		}

		public void BatchSize(int value)
		{
			if (value > 0)
			{
				mapping.batchsize = value;
				mapping.batchsizeSpecified = true;
			}
			else
			{
				mapping.batchsize = 0;
				mapping.batchsizeSpecified = false;
			}
		}

		public void Lazy(CollectionLazy collectionLazy)
		{
			mapping.lazySpecified = true;
			switch (collectionLazy)
			{
				case CollectionLazy.Lazy:
					mapping.lazy = HbmCollectionLazy.True;
					break;
				case CollectionLazy.NoLazy:
					mapping.lazy = HbmCollectionLazy.False;
					break;
				case CollectionLazy.Extra:
					mapping.lazy = HbmCollectionLazy.Extra;
					break;
			}
		}

		public void Key(Action<IKeyMapper> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void OrderBy(MemberInfo property)
		{
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			mapping.orderby = property.Name;
		}

		public void OrderBy(string sqlOrderByClause)
		{
			mapping.orderby = sqlOrderByClause;
		}

		public void Sort() {}
		public void Sort<TComparer>() {}

		public void Cascade(Cascade cascadeStyle)
		{
			mapping.cascade = cascadeStyle.ToCascadeString();
		}

		public void Type<TCollection>() where TCollection : IUserCollectionType
		{
			mapping.collectiontype = typeof (TCollection).AssemblyQualifiedName;
		}

		public void Type(System.Type collectionType)
		{
			if (collectionType == null)
			{
				throw new ArgumentNullException("collectionType");
			}
			if (!typeof (IUserCollectionType).IsAssignableFrom(collectionType))
			{
				throw new ArgumentOutOfRangeException("collectionType",
				                                      string.Format(
				                                      	"The collection type should be an implementation of IUserCollectionType.({0})",
				                                      	collectionType));
			}
			mapping.collectiontype = collectionType.AssemblyQualifiedName;
		}

		public void Table(string tableName)
		{
			mapping.table = tableName;
		}

		public void Catalog(string catalogName)
		{
			mapping.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			mapping.schema = schemaName;
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			if (cacheMapper == null)
			{
				var hbmCache = new HbmCache();
				mapping.cache = hbmCache;
				cacheMapper = new CacheMapper(hbmCache);
			}
			cacheMapping(cacheMapper);
		}

		public void Filter(string filterName, Action<IFilterMapper> filterMapping)
		{
			if (filterMapping == null)
			{
				filterMapping = x => { };
			}
			var hbmFilter = new HbmFilter();
			var filterMapper = new FilterMapper(filterName, hbmFilter);
			filterMapping(filterMapper);
			Dictionary<string, HbmFilter> filters = mapping.filter != null ? mapping.filter.ToDictionary(f => f.name, f => f) : new Dictionary<string, HbmFilter>(1);
			filters[filterName] = hbmFilter;
			mapping.filter = filters.Values.ToArray();
		}

		public void Fetch(CollectionFetchMode fetchMode)
		{
			if (fetchMode == null)
			{
				return;
			}
			mapping.fetch = fetchMode.ToHbm();
			mapping.fetchSpecified = mapping.fetch != HbmCollectionFetchMode.Select;
		}

		public void Persister(System.Type persister)
		{
			if (persister == null)
			{
				throw new ArgumentNullException("persister");
			}
			if (!typeof(ICollectionPersister).IsAssignableFrom(persister))
			{
				throw new ArgumentOutOfRangeException("persister", "Expected type implementing ICollectionPersister.");
			}
			mapping.persister = persister.AssemblyQualifiedName;
		}

		#endregion

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			mapping.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		#region IBagPropertiesMapper Members

		public void Loader(string namedQueryReference)
		{
			if (mapping.SqlLoader == null)
			{
				mapping.loader = new HbmLoader();
			}
			mapping.loader.queryref = namedQueryReference;
		}

		public void SqlInsert(string sql)
		{
			if (mapping.SqlInsert == null)
			{
				mapping.sqlinsert = new HbmCustomSQL();
			}
			mapping.sqlinsert.Text = new[] {sql};
		}

		public void SqlUpdate(string sql)
		{
			if (mapping.SqlUpdate == null)
			{
				mapping.sqlupdate = new HbmCustomSQL();
			}
			mapping.sqlupdate.Text = new[] {sql};
		}

		public void SqlDelete(string sql)
		{
			if (mapping.SqlDelete == null)
			{
				mapping.sqldelete = new HbmCustomSQL();
			}
			mapping.sqldelete.Text = new[] {sql};
		}

		public void SqlDeleteAll(string sql)
		{
			if (mapping.SqlDeleteAll == null)
			{
				mapping.sqldeleteall = new HbmCustomSQL();
			}
			mapping.sqldeleteall.Text = new[] {sql};
		}

		public void Subselect(string sql)
		{
			if (mapping.Subselect == null)
			{
				mapping.subselect = new HbmSubselect();
			}
			mapping.subselect.Text = new[] {sql};
		}

		#endregion
	}
}