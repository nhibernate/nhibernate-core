using System;
using System.Runtime.Serialization;
using System.Security;
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
	public sealed class EntityKey : IDeserializationCallback, ISerializable, IEquatable<EntityKey>
	{
		private readonly object identifier;
		private readonly IEntityPersister _persister;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		[NonSerialized]
		private int? _hashCode;

		/// <summary> Construct a unique identifier for an entity class instance</summary>
		public EntityKey(object id, IEntityPersister persister)
		{
			identifier = id ?? throw new AssertionFailure("null identifier");
			_persister = persister;

			_hashCode = GenerateHashCode();
		}

		private EntityKey(SerializationInfo info, StreamingContext context)
		{
			identifier = info.GetValue(nameof(Identifier), typeof(object));
			var factory = (ISessionFactoryImplementor) info.GetValue(nameof(_persister.Factory), typeof(ISessionFactoryImplementor));
			var entityName = (string) info.GetValue(nameof(EntityName), typeof(string));
			_persister = factory.GetEntityPersister(entityName);
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
				result = 37 * result + RootEntityName.GetHashCode();
				result = 37 * result + _persister.IdentifierType.GetHashCode(identifier, _persister.Factory);
			}
			return result;
		}

		public override string ToString()
		{
			return "EntityKey" + MessageHelper.InfoString(_persister, Identifier, _persister.Factory);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Identifier), identifier);
			info.AddValue(nameof(_persister.Factory), _persister.Factory);
			info.AddValue(nameof(EntityName), EntityName);
		}
	}
}
