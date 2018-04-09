using System;
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
	public class EntityUniqueKey
	{
		private readonly string entityName;
		private readonly string uniqueKeyName;
		private readonly object key;
		private readonly IType keyType;
		private readonly int hashCode;

		// 6.0 TODO: rename semiResolvedKey as simply key. That is not the responsibility of this class to make any
		// assumption on the key being semi-resolved or not, that is the responsibility of its callers.
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
			hashCode = GenerateHashCode(factory);
		}

		public int GenerateHashCode(ISessionFactoryImplementor factory)
		{
			int result = 17;
			unchecked
			{
				result = 37 * result + entityName.GetHashCode();
				result = 37 * result + uniqueKeyName.GetHashCode();
				result = 37 * result + keyType.GetHashCode(key, factory);
			}
			return result;
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
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(this,obj)) return true;
			return Equals(obj as EntityUniqueKey);
		}

		public bool Equals(EntityUniqueKey that)
		{
			return that == null ? false :
				that.EntityName.Equals(entityName) && that.UniqueKeyName.Equals(uniqueKeyName) && keyType.IsEqual(that.key, key);
		}

		public override string ToString()
		{
			return string.Format("EntityUniqueKey{0}", MessageHelper.InfoString(entityName, uniqueKeyName, key));
		}

	}
}
