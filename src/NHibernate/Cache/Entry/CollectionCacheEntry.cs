using System;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Cache.Entry
{
	[Serializable]
	public partial class CollectionCacheEntry
	{
		private readonly object state;

		public static CollectionCacheEntry Create(IPersistentCollection collection, ICollectionPersister persister)
		{
			return new CollectionCacheEntry(collection.Disassemble(persister));
		}

		internal CollectionCacheEntry(object state)
		{
			this.state = state;
		}

		public virtual object[] State
		{
			get { return (object[])state; }//TODO: assumes all collections disassemble to an array!
		}

		public virtual void Assemble(IPersistentCollection collection, ICollectionPersister persister, object owner)
		{
			collection.InitializeFromCache(persister, state, owner);
			collection.AfterInitialize(persister);
		}

		public override string ToString()
		{
			return "CollectionCacheEntry" + ArrayHelper.ToString(State);
		}
	}
}
