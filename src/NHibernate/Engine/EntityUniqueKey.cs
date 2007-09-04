using System;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Used to uniquely key an entity instance in relation to a particular session
	/// by some unique property reference, as opposed to identifier.
	/// Uniqueing information consists of the entity-name, the referenced
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
		private readonly ISessionFactoryImplementor factory;
		private readonly int hashCode;

		public EntityUniqueKey(string entityName, string uniqueKeyName, object semiResolvedKey, 
			IType keyType, ISessionFactoryImplementor factory)
		{
			if (string.IsNullOrEmpty(entityName))
				throw new ArgumentNullException("entityName");
			if (string.IsNullOrEmpty(uniqueKeyName))
				throw new ArgumentNullException("entityName");
			if (semiResolvedKey == null)
				throw new ArgumentNullException("semiResolvedKey");
			if (keyType == null)
				throw new ArgumentNullException("keyType");

			this.entityName = entityName;
			this.uniqueKeyName = uniqueKeyName;
			key = semiResolvedKey;
			this.keyType = keyType;
			this.factory = factory;			
			unchecked
			{
				hashCode = 17;
				hashCode = 37 * hashCode + this.entityName.GetHashCode();
				hashCode = 37 * hashCode + this.uniqueKeyName.GetHashCode();
				hashCode = 37 * hashCode + this.keyType.GetHashCode(key, this.factory);
			}
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
				that.EntityName.Equals(EntityName) && 
				that.UniqueKeyName.Equals(UniqueKeyName) && 
				keyType.Equals(that.Key, Key);
		}

		public override string ToString()
		{
			return string.Format("EntityUniqueKey{0}", MessageHelper.InfoString(entityName, uniqueKeyName, key));
		}

	}
}