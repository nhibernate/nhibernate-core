using System;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// A globally unique identifier of an instance, consisting of the user-visible identifier
	/// and the identifier space (eg. tablename)
	/// </summary>
	[Serializable]
	public sealed class EntityKey
	{
		private readonly object identifier;
		private readonly string rootEntityName;
		private readonly string entityName;
		private readonly IType identifierType;
		private readonly bool isBatchLoadable;

		[NonSerialized]
		private ISessionFactoryImplementor factory;
		private int hashCode;

		private readonly EntityMode entityMode;

		/// <summary> Construct a unique identifier for an entity class instance</summary>
		public EntityKey(object id, IEntityPersister persister, EntityMode entityMode)
			: this(id, persister.RootEntityName, persister.EntityName, persister.IdentifierType, persister.IsBatchLoadable, persister.Factory, entityMode) {}

		/// <summary> Used to reconstruct an EntityKey during deserialization. </summary>
		/// <param name="identifier">The identifier value </param>
		/// <param name="rootEntityName">The root entity name </param>
		/// <param name="entityName">The specific entity name </param>
		/// <param name="identifierType">The type of the identifier value </param>
		/// <param name="batchLoadable">Whether represented entity is eligible for batch loading </param>
		/// <param name="factory">The session factory </param>
		/// <param name="entityMode">The entity's entity mode </param>
		private EntityKey(object identifier, string rootEntityName, string entityName, IType identifierType, bool batchLoadable, ISessionFactoryImplementor factory, EntityMode entityMode)
		{
			if (identifier == null)
				throw new AssertionFailure("null identifier");

			this.identifier = identifier;
			this.rootEntityName = rootEntityName;
			this.entityName = entityName;
			this.identifierType = identifierType;
			isBatchLoadable = batchLoadable;
			this.factory = factory;
			this.entityMode = entityMode;
			hashCode = GenerateHashCode();
		}

		public bool IsBatchLoadable
		{
			get { return isBatchLoadable; }
		}

		public object Identifier
		{
			get { return identifier; }
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public override bool Equals(object other)
		{
			var otherKey = other as EntityKey;
			if(otherKey==null) return false;

			return
				otherKey.rootEntityName.Equals(rootEntityName)
				&& identifierType.IsEqual(otherKey.Identifier, Identifier, entityMode, factory);
		}

		private int GenerateHashCode()
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + rootEntityName.GetHashCode();
				result = 37 * result + identifierType.GetHashCode(identifier, entityMode, factory);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public override string ToString()
		{
			return "EntityKey" + MessageHelper.InfoString(factory.GetEntityPersister(EntityName), Identifier, factory);
		}

		/// <summary>
		/// To use in deserialization callback
		/// </summary>
		/// <param name="sessionFactory"></param>
		internal void SetSessionFactory(ISessionFactoryImplementor sessionFactory)
		{
			factory = sessionFactory;
			hashCode = GetHashCode();
		}
	}
}