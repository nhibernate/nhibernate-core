using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ClassMapper : AbstractPropertyContainerMapper, IClassMapper
	{
		private readonly HbmClass classMapping;
		private readonly IIdMapper idMapper;
		private bool simpleIdPropertyWasUsed;
		private bool composedIdWasUsed;
		private bool componentAsIdWasUsed;
		private Dictionary<string, IJoinMapper> joinMappers;
		private ICacheMapper cacheMapper;
		private IDiscriminatorMapper discriminatorMapper;
		private INaturalIdMapper naturalIdMapper;
		private IVersionMapper versionMapper;

		public ClassMapper(System.Type rootClass, HbmMapping mapDoc, MemberInfo idProperty)
			: base(rootClass, mapDoc)
		{
			classMapping = new HbmClass();
			var toAdd = new[] {classMapping};
			classMapping.name = rootClass.GetShortClassName(mapDoc);
			if (rootClass.IsAbstract)
			{
				classMapping.@abstract = true;
				classMapping.abstractSpecified = true;
			}

			var hbmId = new HbmId();
			classMapping.Item = hbmId;
			idMapper = new IdMapper(idProperty, hbmId);

			mapDoc.Items = mapDoc.Items == null ? toAdd : mapDoc.Items.Concat(toAdd).ToArray();
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] {property};
			classMapping.Items = classMapping.Items == null ? toAdd : classMapping.Items.Concat(toAdd).ToArray();
		}

		#endregion

		public Dictionary<string, IJoinMapper> JoinMappers
		{
			get { return joinMappers ?? (joinMappers = new Dictionary<string, IJoinMapper>()); }
		}

		#region Implementation of IClassMapper
		public void Abstract(bool isAbstract)
		{
			classMapping.@abstract = isAbstract;
			classMapping.abstractSpecified = true;
		}

		public void OptimisticLock(OptimisticLockMode mode)
		{
			classMapping.optimisticlock = (HbmOptimisticLockMode)Enum.Parse(typeof(OptimisticLockMode), mode.ToString());
		}

		public void Id(Action<IIdMapper> mapper)
		{
			mapper(idMapper);
		}

		public void Id(MemberInfo idProperty, Action<IIdMapper> mapper)
		{
			var id = classMapping.Item as HbmId;
			if(id == null)
			{
				var propertyDescription = idProperty != null ? " '" + idProperty.Name + "'" : ", with generator, ";
				throw new MappingException(string.Format("Ambiguous mapping of {0} id. A ComponentAsId or a ComposedId was used and you are trying to map the property{1} as id.",
					Container.FullName, propertyDescription));
			}
			mapper(new IdMapper(idProperty, id));
			if(idProperty != null)
			{
				simpleIdPropertyWasUsed = true;
			}
		}

		public void ComponentAsId(MemberInfo idProperty, Action<IComponentAsIdMapper> mapper)
		{
			if (idProperty == null)
			{
				return;
			}
			if (!IsMemberSupportedByMappedContainer(idProperty))
			{
				throw new ArgumentOutOfRangeException("idProperty", "Can't use, as component id property, a property of another graph");
			}
			if (composedIdWasUsed)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0} id. A composed id was defined and you are trying to map the component {1}, of property '{2}', as id for {0}."
					, Container.FullName, idProperty.GetPropertyOrFieldType().FullName, idProperty.Name));
			}
			if (simpleIdPropertyWasUsed)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0} id. An id property, with generator, was defined and you are trying to map the component {1}, of property '{2}', as id for {0}."
					, Container.FullName, idProperty.GetPropertyOrFieldType().FullName, idProperty.Name));
			}
			var id = classMapping.Item as HbmCompositeId;
			if(id == null)
			{
				id = new HbmCompositeId();
				classMapping.Item = id;
			}
			mapper(new ComponentAsIdMapper(idProperty.GetPropertyOrFieldType(), idProperty, id, mapDoc));
			componentAsIdWasUsed = true;
		}

		public void ComposedId(Action<IComposedIdMapper> idPropertiesMapping)
		{
			if(componentAsIdWasUsed)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0} id. A Component as id was used and you are trying to map an id composed by various properties of {0}.", Container.FullName));
			}
			if (simpleIdPropertyWasUsed)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0} id. An id property, with generator, was defined and you are trying to map an id composed by various properties of {0}.", Container.FullName));
			}

			var id = classMapping.Item as HbmCompositeId;
			if (id == null)
			{
				id = new HbmCompositeId();
				classMapping.Item = id;
			}
			idPropertiesMapping(new ComposedIdMapper(Container, id, mapDoc));
			composedIdWasUsed = true;
		}

		public void Discriminator(Action<IDiscriminatorMapper> discriminatorMapping)
		{
			if (discriminatorMapper == null)
			{
				var hbmDiscriminator = new HbmDiscriminator();
				classMapping.discriminator = hbmDiscriminator;
				discriminatorMapper = new DiscriminatorMapper(hbmDiscriminator);
			}
			discriminatorMapping(discriminatorMapper);
		}

		public void DiscriminatorValue(object value)
		{
			if (value != null)
			{
				classMapping.discriminatorvalue = value.ToString();
				Discriminator(x => { });
				System.Type valueType = value.GetType();
				if (valueType != typeof (string))
				{
					classMapping.discriminator.type = valueType.GetNhTypeName();
				}
			}
			else
			{
				classMapping.discriminatorvalue = "null";
			}
		}

		public void Table(string tableName)
		{
			classMapping.table = tableName;
		}

		public void Check(string check)
		{
			classMapping.check = check;
		}

		public void Catalog(string catalogName)
		{
			classMapping.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			classMapping.schema = schemaName;
		}

		public void Mutable(bool isMutable)
		{
			classMapping.mutable = isMutable;
		}

		public void Version(MemberInfo versionProperty, Action<IVersionMapper> versionMapping)
		{
			if (versionMapper == null)
			{
				var hbmVersion = new HbmVersion();
				classMapping.Item1 = hbmVersion;
				versionMapper = new VersionMapper(versionProperty, hbmVersion);
			}
			versionMapping(versionMapper);
		}

		public void NaturalId(Action<INaturalIdMapper> naturalIdMapping)
		{
			if (naturalIdMapper == null)
			{
				naturalIdMapper = new NaturalIdMapper(Container, classMapping, MapDoc);
			}
			naturalIdMapping(naturalIdMapper);
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			if (cacheMapper == null)
			{
				var hbmCache = new HbmCache();
				classMapping.cache = hbmCache;
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
			Dictionary<string, HbmFilter> filters = classMapping.filter != null ? classMapping.filter.ToDictionary(f => f.name, f => f) : new Dictionary<string, HbmFilter>(1);
			filters[filterName] = hbmFilter;
			classMapping.filter = filters.Values.ToArray();
		}

		public void Where(string whereClause)
		{
			classMapping.where = whereClause;
		}

		public void SchemaAction(SchemaAction action)
		{
			classMapping.schemaaction = action.ToSchemaActionString();
		}

		public void Join(string splitGroupId, Action<IJoinMapper> splitMapping)
		{
			IJoinMapper splitGroup;
			if(!JoinMappers.TryGetValue(splitGroupId, out splitGroup))
			{
				var hbmJoin = new HbmJoin();
				splitGroup = new JoinMapper(Container, splitGroupId, hbmJoin, MapDoc);
				var toAdd = new[] { hbmJoin };
				JoinMappers.Add(splitGroupId, splitGroup);
				classMapping.Items1 = classMapping.Items1 == null ? toAdd : classMapping.Items1.Concat(toAdd).ToArray();
			}

			splitMapping(splitGroup);
		}

		public void Polymorphism(PolymorphismType type)
		{
			classMapping.polymorphism = (HbmPolymorphismType) Enum.Parse(typeof(HbmPolymorphismType), type.ToString());
		}
		#endregion

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			classMapping.entityname = value;
		}

		public void Proxy(System.Type proxy)
		{
			if (!Container.IsAssignableFrom(proxy) && !proxy.IsAssignableFrom(Container))
			{
				throw new MappingException("Not compatible proxy for " + Container);
			}
			classMapping.proxy = proxy.GetShortClassName(MapDoc);
		}

		public void Lazy(bool value)
		{
			classMapping.lazy = value;
			classMapping.lazySpecified = !value;
		}

		public void DynamicUpdate(bool value)
		{
			classMapping.dynamicupdate = value;
		}

		public void DynamicInsert(bool value)
		{
			classMapping.dynamicinsert = value;
		}

		public void BatchSize(int value)
		{
			if (value > 0)
			{
				classMapping.batchsize = value;
				classMapping.batchsizeSpecified = true;
			}
			else
			{
				classMapping.batchsize = 0;
				classMapping.batchsizeSpecified = false;
			}
		}

		public void SelectBeforeUpdate(bool value)
		{
			classMapping.selectbeforeupdate = value;
		}

		public void Persister<T>() where T : IEntityPersister
		{
			classMapping.persister = typeof (T).GetShortClassName(MapDoc);
		}

		public void Synchronize(params string[] table)
		{
			if (table == null)
			{
				return;
			}
			var existingSyncs = new HashSet<string>(classMapping.synchronize != null ? classMapping.synchronize.Select(x => x.table) : Enumerable.Empty<string>());
			System.Array.ForEach(table.Where(x => x != null).Select(tableName => tableName.Trim()).Where(cleanedName => !"".Equals(cleanedName)).ToArray(),
			                     x => existingSyncs.Add(x.Trim()));
			classMapping.synchronize = existingSyncs.Select(x => new HbmSynchronize {table = x}).ToArray();
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			if (classMapping.SqlLoader == null)
			{
				classMapping.loader = new HbmLoader();
			}
			classMapping.loader.queryref = namedQueryReference;
		}

		public void SqlInsert(string sql)
		{
			if (classMapping.SqlInsert == null)
			{
				classMapping.sqlinsert = new HbmCustomSQL();
			}
			classMapping.sqlinsert.Text = new[] {sql};
		}

		public void SqlUpdate(string sql)
		{
			if (classMapping.SqlUpdate == null)
			{
				classMapping.sqlupdate = new HbmCustomSQL();
			}
			classMapping.sqlupdate.Text = new[] {sql};
		}

		public void SqlDelete(string sql)
		{
			if (classMapping.SqlDelete == null)
			{
				classMapping.sqldelete = new HbmCustomSQL();
			}
			classMapping.sqldelete.Text = new[] {sql};
		}

		public void Subselect(string sql)
		{
			if (classMapping.Subselect == null)
			{
				classMapping.subselect = new HbmSubselect();
			}
			classMapping.subselect.Text = new[] {sql};
		}

		#endregion
	}
}