using System;
using System.Collections.Generic;
using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class SubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, ISubclassMapper<TEntity>, IConformistHoldersProvider where TEntity : class
	{
		private Dictionary<string, IJoinMapper<TEntity>> joinCustomizers;

		public SubclassCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsTablePerClassHierarchyEntity(typeof (TEntity));
			
			// Add an empty customizer as a way to register the class as explicity declared
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => { });
		}

		#region ISubclassMapper<TEntity> Members
		public void Extends(System.Type baseType)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Extends(baseType));
		}

		public void Abstract(bool isAbstract)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Abstract(isAbstract));
		}

		public void DiscriminatorValue(object value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.DiscriminatorValue(value));
		}

		public void Filter(string filterName, Action<IFilterMapper> filterMapping)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Filter(filterName, filterMapping));
		}

		#endregion

		private Dictionary<string, IJoinMapper<TEntity>> JoinCustomizers
		{
			get { return joinCustomizers ?? (joinCustomizers = new Dictionary<string, IJoinMapper<TEntity>>()); }
		}

		public void Join(string splitGroupId, Action<IJoinMapper<TEntity>> splitMapping)
		{
			// add the customizer only to ensure the creation of the JoinMapper instance for the group
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Join(splitGroupId, j => { }));

			IJoinMapper<TEntity> joinCustomizer;
			if (!JoinCustomizers.TryGetValue(splitGroupId, out joinCustomizer))
			{
				joinCustomizer = new JoinCustomizer<TEntity>(splitGroupId, ExplicitDeclarationsHolder, CustomizersHolder);
				JoinCustomizers.Add(splitGroupId, joinCustomizer);
			}
			splitMapping(joinCustomizer);
		}

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.EntityName(value));
		}

		public void Proxy(System.Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.Persister<T>());
		}

		public void Synchronize(params string[] table)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Synchronize(table));
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.SqlDelete(sql));
		}

		public void Subselect(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (ISubclassMapper m) => m.Subselect(sql));
		}

		#endregion

		#region IConformistHoldersProvider Members

		ICustomizersHolder IConformistHoldersProvider.CustomizersHolder
		{
			get { return CustomizersHolder; }
		}

		IModelExplicitDeclarationsHolder IConformistHoldersProvider.ExplicitDeclarationsHolder
		{
			get { return ExplicitDeclarationsHolder; }
		}

		#endregion
	}
}