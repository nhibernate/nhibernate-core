using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class SubclassMapper : AbstractPropertyContainerMapper, ISubclassMapper
	{
		private readonly HbmSubclass classMapping = new HbmSubclass();
		private Dictionary<string, IJoinMapper> joinMappers;

		public SubclassMapper(System.Type subClass, HbmMapping mapDoc) : base(subClass, mapDoc)
		{
			var toAdd = new[] {classMapping};
			classMapping.name = subClass.GetShortClassName(mapDoc);
			classMapping.extends = subClass.BaseType.GetShortClassName(mapDoc);
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

		#region ISubclassMapper Members
		public void Abstract(bool isAbstract)
		{
			classMapping.@abstract = isAbstract;
			classMapping.abstractSpecified = true;
		}

		public void DiscriminatorValue(object value)
		{
			classMapping.discriminatorvalue = value != null ? value.ToString() : "null";
		}

		public void Extends(System.Type baseType)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (!Container.GetBaseTypes().Contains(baseType))
			{
				throw new ArgumentOutOfRangeException("baseType",
				                                      string.Format("{0} is a valid super-class of {1}", baseType, Container));
			}
			classMapping.extends = baseType.GetShortClassName(MapDoc);
		}

		public void Join(string splitGroupId, Action<IJoinMapper> splitMapping)
		{
			IJoinMapper splitGroup;
			if (!JoinMappers.TryGetValue(splitGroupId, out splitGroup))
			{
				var hbmJoin = new HbmJoin();
				splitGroup = new JoinMapper(Container, splitGroupId, hbmJoin, MapDoc);
				var toAdd = new[] { hbmJoin };
				JoinMappers.Add(splitGroupId, splitGroup);
				classMapping.join = classMapping.join == null ? toAdd : classMapping.join.Concat(toAdd).ToArray();
			}

			splitMapping(splitGroup);
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
			classMapping.batchsize = value > 0 ? value.ToString() : null;
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
			classMapping.synchronize = existingSyncs.Select(x => new HbmSynchronize { table = x }).ToArray();
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

		public void Subselect(string sql) {}

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
		#endregion
	}
}