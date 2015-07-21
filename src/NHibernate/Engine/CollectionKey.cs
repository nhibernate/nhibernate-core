using System;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Uniquely identifies a collection instance in a particular session. 
	/// </summary>
	[Serializable]
	public sealed class CollectionKey
	{
		private readonly string role;
		private readonly object key;
		private readonly IType keyType;
		[NonSerialized] private readonly ISessionFactoryImplementor factory;
		private readonly int hashCode;
		private readonly EntityMode entityMode;

		public CollectionKey(ICollectionPersister persister, object key, EntityMode entityMode)
			: this(persister.Role, key, persister.KeyType, entityMode, persister.Factory)
		{
		}

		private CollectionKey(string role, object key, IType keyType, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			this.role = role;
			this.key = key;
			this.keyType = keyType;
			this.entityMode = entityMode;
			this.factory = factory;
			hashCode = GenerateHashCode(); //cache the hashcode
		}

		public override bool Equals(object obj)
		{
			CollectionKey that = (CollectionKey)obj;
			return that.role.Equals(role) && keyType.IsEqual(that.key, key, entityMode, factory);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		private int GenerateHashCode()
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + role.GetHashCode();
				result = 37 * result + keyType.GetHashCode(key, entityMode, factory);
			}
			return result;
		}

		public string Role
		{
			get { return role; }
		}

		public object Key
		{
			get { return key; }
		}

		public override string ToString()
		{
			return "CollectionKey" + MessageHelper.CollectionInfoString(factory.GetCollectionPersister(role), key, factory);
		}
	}
}