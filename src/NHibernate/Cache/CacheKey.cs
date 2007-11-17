using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// Allows multiple entity classes / collection roles to be 
	/// stored in the same cache region. Also allows for composite 
	/// keys which do not properly implement equals()/hashCode().
	/// </summary>
	[Serializable]
	public class CacheKey
	{
		private object key;
		private IType type;
		private string entityOrRoleName;
		private ISessionFactoryImplementor factory;
		private int hashCode;

		/// <summary>
		/// Construct a new key for a collection or entity instance.
		/// Note that an entity name should always be the root entity 
		/// name, not a subclass entity name.
		/// </summary>
		public CacheKey(object id, IType type, string entityOrRoleName, ISessionFactoryImplementor factory)
		{
			key = id;
			this.type = type;
			this.entityOrRoleName = entityOrRoleName;
			this.factory = factory;
			hashCode = type.GetHashCode(key, factory);
		}

		//Mainly for SysCache and Memcache
		public override String ToString()
		{
			if (type is ComponentType)
				return entityOrRoleName + '#' + type.ToLoggableString(key, factory);
			else
				return entityOrRoleName + '#' + key.ToString();
		}

		public override bool Equals(Object other)
		{
			if (!(other is CacheKey)) return false;
			CacheKey that = (CacheKey) other;
			return type.Equals(key, that.key) && entityOrRoleName.Equals(that.entityOrRoleName);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public object Key
		{
			get { return key; }
		}

		public string EntityOrRoleName
		{
			get { return entityOrRoleName; }
		}
	}
}