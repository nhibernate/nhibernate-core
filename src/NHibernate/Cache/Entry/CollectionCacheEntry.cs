using System;
using System.Runtime.Serialization;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Cache.Entry
{
	[Serializable]
	[DataContract]
	public partial class CollectionCacheEntry
	{
		private object state;

		public CollectionCacheEntry()
		{
		}

		// Since 5.2
		[Obsolete("Use CollectionCacheEntry.Create method instead.")]
		public CollectionCacheEntry(IPersistentCollection collection, ICollectionPersister persister)
		{
			state = collection.Disassemble(persister);
		}

		public static CollectionCacheEntry Create(IPersistentCollection collection, ICollectionPersister persister)
		{
			return new CollectionCacheEntry
			{
				state = collection.Disassemble(persister)
			};
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public virtual object[] State
		{
			get => (object[]) state; //TODO: assumes all collections disassemble to an array!
			set => state = value;
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
