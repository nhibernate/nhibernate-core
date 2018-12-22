using System;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	/// <summary> Represents some non-scalar (entity/collection) return within the query result. </summary>
	internal class NonScalarReturn : IReturn
	{
		private readonly string alias;
		private readonly LockMode lockMode;
		private readonly ISqlLoadableCollection _collectionPersister;

		public NonScalarReturn(SQLQueryContext context, bool queryHasAliases, string alias, LockMode lockMode)
			: this(context, queryHasAliases, alias, lockMode, null)
		{}

		public NonScalarReturn(SQLQueryContext context, bool queryHasAliases, string alias, LockMode lockMode, NonScalarReturn owner)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (string.IsNullOrEmpty(alias))
			{
				throw new HibernateException("alias must be specified");
			}

			this.alias = alias;
			this.lockMode = lockMode;
			Owner = owner;

			_collectionPersister = context.GetCollectionPersister(alias);
			if (_collectionPersister != null)
			{
				var collectionPropertyResultMap = context.GetCollectionPropertyResultsMap(alias);
				CollectionAliases = queryHasAliases
					? new GeneratedCollectionAliases(collectionPropertyResultMap, _collectionPersister, context.GetCollectionSuffix(alias))
					: (ICollectionAliases)new ColumnCollectionAliases(collectionPropertyResultMap, _collectionPersister);
			}

			if (_collectionPersister == null || CollectionPersister.ElementType.IsEntityType)
			{
				EntityPersister = context.GetEntityPersister(alias);
				if (EntityPersister != null)
				{
					var entityPropertyResultMap = context.GetEntityPropertyResultsMap(alias);
					EntityAliases = queryHasAliases
						? new DefaultEntityAliases(entityPropertyResultMap, EntityPersister, context.GetEntitySuffix(alias))
						: new ColumnEntityAliases(entityPropertyResultMap, EntityPersister);
				}
			}
		}

		public string Alias
		{
			get { return alias; }
		}

		public LockMode LockMode
		{
			get { return lockMode; }
		}

		public NonScalarReturn Owner { get; }

		public ICollectionPersister CollectionPersister => _collectionPersister;

		public ICollectionAliases CollectionAliases { get; }

		public ISqlLoadable EntityPersister { get; }

		public IEntityAliases EntityAliases { get; }

		public IType Type
		{
			get
			{
				if (_collectionPersister != null)
					return _collectionPersister.CollectionType;
				return EntityPersister?.EntityMetamodel.EntityType;
			}
		}
	}
}
