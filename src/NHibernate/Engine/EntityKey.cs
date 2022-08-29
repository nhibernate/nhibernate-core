using System;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Impl;
using NHibernate.Persister.Entity;

namespace NHibernate.Engine
{
	//TODO 6.0: Remove IDeserializationCallback interface
	/// <summary>
	/// A globally unique identifier of an instance, consisting of the user-visible identifier
	/// and the identifier space (eg. tablename)
	/// </summary>
	[Serializable]
	public sealed class EntityKey : IDeserializationCallback, ISerializable, IEquatable<EntityKey>
	{
		private readonly object identifier;
		private readonly IEntityPersister _persister;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		private readonly int _hashCode;

		/// <summary> Construct a unique identifier for an entity class instance</summary>
		public EntityKey(object id, IEntityPersister persister)
		{
			identifier = id ?? throw new AssertionFailure("null identifier");
			_persister = persister;
			_hashCode = GenerateHashCode(persister, id);
		}

		private EntityKey(SerializationInfo info, StreamingContext context)
		{
			identifier = info.GetValue(nameof(Identifier), typeof(object));
			var factory = (ISessionFactoryImplementor) info.GetValue(nameof(_persister.Factory), typeof(ISessionFactoryImplementor));
			var entityName = (string) info.GetValue(nameof(EntityName), typeof(string));
			_persister = factory.GetEntityPersister(entityName);
			_hashCode = GenerateHashCode(_persister, identifier);
		}

		public bool IsBatchLoadable => _persister.IsBatchLoadable;

		public object Identifier
		{
			get { return identifier; }
		}

		public string EntityName => _persister.EntityName;

		internal string RootEntityName => _persister.RootEntityName;

		public override bool Equals(object other)
		{
			return other is EntityKey otherKey && Equals(otherKey);
		}

		public bool Equals(EntityKey other)
		{
			if (other == null)
			{
				return false;
			}

			return
				other.RootEntityName.Equals(RootEntityName)
				&& _persister.IdentifierType.IsEqual(other.Identifier, Identifier, _persister.Factory);
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}

		private static int GenerateHashCode(IEntityPersister persister, object id)
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + persister.RootEntityName.GetHashCode();
				result = 37 * result + persister.IdentifierType.GetHashCode(id, persister.Factory);
			}
			return result;
		}

		public override string ToString()
		{
			return "EntityKey" + MessageHelper.InfoString(_persister, Identifier, _persister.Factory);
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Identifier), identifier);
			info.AddValue(nameof(_persister.Factory), _persister.Factory);
			info.AddValue(nameof(EntityName), EntityName);
		}

		[Obsolete("IDeserializationCallback interface has no usages and will be removed in a future version")]
		public void OnDeserialization(object sender)
		{ }
	}
}
