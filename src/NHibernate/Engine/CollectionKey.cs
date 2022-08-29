using System;
using System.Runtime.Serialization;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Uniquely identifies a collection instance in a particular session. 
	/// </summary>
	[Serializable]
	public sealed class CollectionKey : IDeserializationCallback
	{
		private readonly string role;
		private readonly object key;
		private readonly IType keyType;
		private readonly ISessionFactoryImplementor factory;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		[NonSerialized]
		private int? _hashCode;

		public CollectionKey(ICollectionPersister persister, object key)
			: this(persister.Role, key, persister.KeyType, persister.Factory)
		{
		}

		private CollectionKey(string role, object key, IType keyType, ISessionFactoryImplementor factory)
		{
			this.role = role;
			this.key = key;
			this.keyType = keyType;
			this.factory = factory;

			_hashCode = GenerateHashCode();
		}

		public override bool Equals(object obj)
		{
			CollectionKey that = (CollectionKey) obj;
			return that.role.Equals(role) && keyType.IsEqual(that.key, key, factory);
		}

		public override int GetHashCode()
		{
			// If the object is put in a set or dictionary during deserialization, the hashcode will not yet be
			// computed. Compute the hashcode on the fly. So long as this happens only during deserialization, there
			// will be no thread safety issues. For the hashcode to be always defined after deserialization, the
			// deserialization callback is used.
			return _hashCode ?? GenerateHashCode();
		}

		/// <inheritdoc />
		public void OnDeserialization(object sender)
		{
			_hashCode = GenerateHashCode();
		}

		private int GenerateHashCode()
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + role.GetHashCode();
				result = 37 * result + keyType.GetHashCode(key, factory);
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
