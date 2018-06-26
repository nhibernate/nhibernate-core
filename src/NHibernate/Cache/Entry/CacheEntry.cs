using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using NHibernate.Properties;
using NHibernate.Type;

namespace NHibernate.Cache.Entry
{
	/// <summary>
	/// A cached instance of a persistent class
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(AnyType.ObjectTypeCacheEntry))]
	[XmlInclude(typeof(UnfetchedLazyProperty))]
	[XmlInclude(typeof(UnknownBackrefProperty))]
	[DataContract]
	[KnownType(typeof(AnyType.ObjectTypeCacheEntry))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(UnfetchedLazyProperty))]
	[KnownType(typeof(UnknownBackrefProperty))]
	public sealed partial class CacheEntry
	{
		private object[] disassembledState;
		private string subclass;
		private bool lazyPropertiesAreUnfetched;
		private object version;

		public CacheEntry()
		{
		}

		// Since 5.2
		[Obsolete("Please use CacheEntry.Create method instead.")]
		public CacheEntry(object[] state, IEntityPersister persister, bool unfetched, object version, ISessionImplementor session, object owner)
		{
			//disassembled state gets put in a new array (we write to cache by value!)
			DisassembledState = TypeHelper.Disassemble(state, persister.PropertyTypes, null, session, owner);
			Subclass = persister.EntityName;
			AreLazyPropertiesUnfetched = unfetched || !persister.IsLazyPropertiesCacheable;
			Version = version;
		}

		public static CacheEntry Create(object[] state, IEntityPersister persister, bool unfetched, object version,
										ISessionImplementor session, object owner)
		{
			return new CacheEntry
			{
				//disassembled state gets put in a new array (we write to cache by value!)
				DisassembledState = TypeHelper.Disassemble(state, persister.PropertyTypes, null, session, owner),
				AreLazyPropertiesUnfetched = unfetched || !persister.IsLazyPropertiesCacheable,
				Subclass = persister.EntityName,
				Version = version
			};
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public object Version
		{
			get => version;
			set => version = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public string Subclass
		{
			get => subclass;
			set => subclass = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public bool AreLazyPropertiesUnfetched
		{
			get => lazyPropertiesAreUnfetched;
			set => lazyPropertiesAreUnfetched = value;
		}

		// todo: this was added to support initializing an entity's EntityEntry snapshot during reattach;
		// this should be refactored to instead expose a method to assemble a EntityEntry based on this
		// state for return.
		// 6.0 TODO convert to auto-property
		[DataMember]
		public object[] DisassembledState
		{
			get => disassembledState;
			set => disassembledState = value;
		}

		public object[] Assemble(object instance, object id, IEntityPersister persister, IInterceptor interceptor,
		                         ISessionImplementor session)
		{
			if (!persister.EntityName.Equals(Subclass))
			{
				throw new AssertionFailure("Tried to assemble a different subclass instance");
			}

			return Assemble(DisassembledState, instance, id, persister, interceptor, session);
		}

		private static object[] Assemble(object[] values, object result, object id, IEntityPersister persister,
		                                 IInterceptor interceptor, ISessionImplementor session)
		{
			//assembled state gets put in a new array (we read from cache by value!)
			object[] assembledProps = TypeHelper.Assemble(values, persister.PropertyTypes, session, result);
	
			//from h3.2 TODO: reuse the PreLoadEvent
			PreLoadEvent preLoadEvent = new PreLoadEvent((IEventSource) session);
			preLoadEvent.Entity = result;
			preLoadEvent.State=assembledProps;
			preLoadEvent.Id = id;
			preLoadEvent.Persister=persister;

			IPreLoadEventListener[] listeners = session.Listeners.PreLoadEventListeners;
			for (int i = 0; i < listeners.Length; i++)
			{
				listeners[i].OnPreLoad(preLoadEvent);
			}

			persister.SetPropertyValues(result, assembledProps);

			return assembledProps;
		}
	}
}
