using System;
using System.Runtime.Serialization;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Used to uniquely key an entity instance in relation to a particular session
	/// by some unique property reference, as opposed to identifier.
	/// Unique information consists of the entity-name, the referenced
	/// property name, and the referenced property value. 
	/// </summary>
	/// <seealso cref="EntityKey"/>
	[Serializable]
	public class EntityUniqueKey : IDeserializationCallback
	{
		private readonly string entityName;
		private readonly string uniqueKeyName;
		private readonly object key;
		private readonly IType keyType;
		private readonly ISessionFactoryImplementor _factory;
		// hashcode may vary among processes, they cannot be stored and have to be re-computed after deserialization
		[NonSerialized]
		private int? _hashCode;

		// 6.0 TODO: rename keyType as semiResolvedKeyType. That is not the responsibility of this class to make any
		// assumption on the key type being semi-resolved or not, that is the responsibility of its callers.
		public EntityUniqueKey(string entityName, string uniqueKeyName, object semiResolvedKey, IType keyType, ISessionFactoryImplementor factory)
		{
			if (string.IsNullOrEmpty(entityName))
				throw new ArgumentNullException("entityName");
			if (string.IsNullOrEmpty(uniqueKeyName))
				throw new ArgumentNullException("uniqueKeyName");
			if (semiResolvedKey == null)
				throw new ArgumentNullException("semiResolvedKey");
			if (keyType == null)
				throw new ArgumentNullException("keyType");

			this.entityName = entityName;
			this.uniqueKeyName = uniqueKeyName;
			key = semiResolvedKey;
			this.keyType = keyType;
			_factory = factory;

			// No need to delay computation here, but we need the lazy for the deserialization case.
			_hashCode = GenerateHashCode();
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public object Key
		{
			get { return key; }
		}

		public string UniqueKeyName
		{
			get { return uniqueKeyName; }
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

		public int GenerateHashCode()
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + entityName.GetHashCode();
				result = 37 * result + uniqueKeyName.GetHashCode();
				result = 37 * result + keyType.GetHashCode(key, _factory);
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as EntityUniqueKey);
		}

		public bool Equals(EntityUniqueKey that)
		{
			return that != null && that.EntityName.Equals(entityName) && that.UniqueKeyName.Equals(uniqueKeyName) &&
				// Normally entities are cached by semi-resolved type only, but the initial fix of #1226 causes them to
				// be cached by type too. This may then cause issues (including Stack Overflow Exception) when this
				// happens with the that.keyType being an entity type while its value is an uninitialized proxy: if
				// this.keyType is not an entity type too, its IsEqual will trigger the proxy loading.
				// So we need to short-circuit on keyType inequality, at least till Loader.CacheByUniqueKey is removed.
				// 6.0 TODO: consider removing the keyType.Equals(that.keyType) check, see above comment.
				keyType.Equals(that.keyType) &&
				keyType.IsEqual(that.key, key);
		}

		public override string ToString()
		{
			return string.Format("EntityUniqueKey{0}", MessageHelper.InfoString(entityName, uniqueKeyName, key));
		}
	}
}
