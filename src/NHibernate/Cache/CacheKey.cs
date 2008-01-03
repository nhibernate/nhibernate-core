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
		private readonly object key;
		private readonly IType type;
		private readonly string entityOrRoleName;
		private readonly ISessionFactoryImplementor factory;
		private readonly int hashCode;

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
			hashCode = type.GetHashCode(key, EntityMode.Poco, factory);
		}

		//Mainly for SysCache and Memcache
		public override String ToString()
		{
			if (type is ComponentType)
				return entityOrRoleName + '#' + type.ToLoggableString(key, factory);
			else
				return entityOrRoleName + '#' + key;
		}

		public override bool Equals(Object other)
		{
			if (!(other is CacheKey)) return false;
			CacheKey that = (CacheKey) other;
			return entityOrRoleName.Equals(that.entityOrRoleName) && type.IsEqual(key, that.key, EntityMode.Poco);
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