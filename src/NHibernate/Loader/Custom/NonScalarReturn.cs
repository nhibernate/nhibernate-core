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
		private readonly NonScalarReturn owner;
		private readonly ISqlLoadable entityPersister;
		private readonly IEntityAliases entityAliases;
		private readonly ISqlLoadableCollection collectionPersister;
		private readonly ICollectionAliases collectionAliases;

		public NonScalarReturn(SQLQueryContext context, bool queryHasAliases, string alias, LockMode lockMode)
			: this(context, queryHasAliases, alias, lockMode, null)
		{}

		public NonScalarReturn(SQLQueryContext context, bool queryHasAliases, string alias, LockMode lockMode, NonScalarReturn owner)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (string.IsNullOrEmpty(alias))
			{
				throw new HibernateException("alias must be specified");
			}

			this.alias = alias;
			this.lockMode = lockMode;
			this.owner = owner;

			this.collectionPersister = context.GetCollectionPersister(alias);
			if (this.collectionPersister != null)
			{
				var collectionPropertyResultMap = context.GetCollectionPropertyResultsMap(alias);
				this.collectionAliases = queryHasAliases
					? new GeneratedCollectionAliases(collectionPropertyResultMap, this.collectionPersister, context.GetCollectionSuffix(alias))
					: (ICollectionAliases)new ColumnCollectionAliases(collectionPropertyResultMap, this.collectionPersister);
			}

			if (this.collectionPersister == null || this.CollectionPersister.ElementType.IsEntityType)
			{
				this.entityPersister = context.GetEntityPersister(alias);
				if (this.entityPersister != null)
				{
					var entityPropertyResultMap = context.GetEntityPropertyResultsMap(alias);
					this.entityAliases = queryHasAliases
						? new DefaultEntityAliases(entityPropertyResultMap, this.entityPersister, context.GetEntitySuffix(alias))
						: new ColumnEntityAliases(entityPropertyResultMap, this.entityPersister);
				}
			}
		}

		public string Alias
		{
			get { return this.alias; }
		}

		public LockMode LockMode
		{
			get { return this.lockMode; }
		}

		public NonScalarReturn Owner
		{
			get { return this.owner; }
		}

		public ICollectionPersister CollectionPersister
		{
			get { return this.collectionPersister; }
		}

		public ICollectionAliases CollectionAliases
		{
			get { return this.collectionAliases; }
		}

		public ISqlLoadable EntityPersister
		{
			get { return this.entityPersister; }
		}

		public IEntityAliases EntityAliases
		{
			get { return this.entityAliases; }
		}

		public IType Type
		{
			get
			{
				if (this.collectionPersister != null) return this.collectionPersister.CollectionType;
				if (this.entityPersister != null) return this.entityPersister.EntityMetamodel.EntityType;
				return null;
			}
		}
	}
}