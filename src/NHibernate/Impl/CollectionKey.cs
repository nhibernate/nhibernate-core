using System;
using System.Collections;

using NHibernate.Collection;

namespace NHibernate.Impl
{
	[Serializable]
	internal sealed class CollectionKey
	{
		private string role;
		private object key;

		public CollectionKey(string role, object key)
		{
			this.role = role;
			this.key  = key;
		}

		public CollectionKey(CollectionPersister persister, object key)
			: this(persister.Role, key)
		{
		}

		public override bool Equals(object obj)
		{
			CollectionKey that = (CollectionKey) obj;
			return Equals (key, that.key)
				&& Equals (role, that.role);
		}

		public override int GetHashCode()
		{
			int result = 17;
			result = 37 * result + key.GetHashCode();
			result = 37 * result + role.GetHashCode();
			return result;
		}
	}
}
