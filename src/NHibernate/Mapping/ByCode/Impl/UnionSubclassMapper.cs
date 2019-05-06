using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class UnionSubclassMapper : AbstractPropertyContainerMapper, IUnionSubclassMapper
	{
		private readonly HbmUnionSubclass classMapping;

		public UnionSubclassMapper(System.Type subClass, HbmMapping mapDoc)
			: base(subClass, mapDoc)
		{
			classMapping = new HbmUnionSubclass();
			classMapping.name = subClass.GetShortClassName(mapDoc);
			classMapping.extends = subClass.BaseType.GetShortClassName(mapDoc);
			if (subClass.IsAbstract)
			{
				classMapping.@abstract = true;
				classMapping.abstractSpecified = true;
			}
			mapDoc.Items = ArrayHelper.Append(mapDoc.Items, classMapping);
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}

			classMapping.Items = ArrayHelper.Append(classMapping.Items, property);
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

		public void Subselect(string sql)
		{
			if (classMapping.Subselect == null)
			{
				classMapping.subselect = new HbmSubselect();
			}
			classMapping.subselect.Text = new[] {sql};
		}

		#endregion

		#region Implementation of IUnionSubclassAttributesMapper
		public void Abstract(bool isAbstract)
		{
			classMapping.@abstract = isAbstract;
			classMapping.abstractSpecified = true;
		}

		public void Table(string tableName)
		{
			classMapping.table = tableName;
		}

		public void Catalog(string catalogName)
		{
			classMapping.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			classMapping.schema = schemaName;
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

		public void Extends(string entityOrClassName)
		{
			classMapping.extends = entityOrClassName ?? throw new ArgumentNullException(nameof(entityOrClassName));
		}

		#endregion
	}
}
