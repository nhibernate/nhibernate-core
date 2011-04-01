using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class JoinedSubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IJoinedSubclassMapper<TEntity> where TEntity : class
	{
		private readonly IKeyMapper<TEntity> keyMapper;

		public JoinedSubclassCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsTablePerClassEntity(typeof (TEntity));
			keyMapper = new JoinedSubclassKeyCustomizer<TEntity>(customizersHolder);
		}

		#region Implementation of IEntityAttributesMapper

		public void Key(Action<IKeyMapper<TEntity>> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.EntityName(value));
		}

		public void Proxy(System.Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Persister<T>());
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.SqlDelete(sql));
		}

		public void Subselect(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Subselect(sql));
		}

		#endregion

		#region Implementation of IJoinedSubclassAttributesMapper<TEntity>

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Schema(schemaName));
		}

		#endregion
	}
}