using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NHibernate.Collection;
using NHibernate.Intercept;
using NHibernate.Persister.Collection;
using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache.Entry
{
	[Serializable]
	[XmlInclude(typeof(AnyType.ObjectTypeCacheEntry))]
	[XmlInclude(typeof(object[]))]
	[XmlInclude(typeof(UnfetchedLazyProperty))]
	[XmlInclude(typeof(UnknownBackrefProperty))]
	[DataContract]
	[KnownType(typeof(AnyType.ObjectTypeCacheEntry))]
	[KnownType(typeof(object[]))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(UnfetchedLazyProperty))]
	[KnownType(typeof(UnknownBackrefProperty))]
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
			DisassembledState = collection.Disassemble(persister);
		}

		public static CollectionCacheEntry Create(IPersistentCollection collection, ICollectionPersister persister)
		{
			return new CollectionCacheEntry
			{
				DisassembledState = collection.Disassemble(persister)
			};
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public object DisassembledState
		{
			get => state;
			set => state = value;
		}

		//TODO: assumes all collections disassemble to an array!
		[Obsolete("Please use DisassembledState property instead.")]
		public virtual object[] State => (object[]) DisassembledState;

		public virtual void Assemble(IPersistentCollection collection, ICollectionPersister persister, object owner)
		{
			collection.InitializeFromCache(persister, DisassembledState, owner);
			collection.AfterInitialize(persister);
		}

		public override string ToString()
		{
			if (DisassembledState is object[] array)
			{
				return "CollectionCacheEntry" + ArrayHelper.ToString(array);
			}
			return "CollectionCacheEntry" + StringHelper.Unqualify(DisassembledState.GetType().FullName) + "@" +
			       DisassembledState.GetHashCode();
		}
	}
}
