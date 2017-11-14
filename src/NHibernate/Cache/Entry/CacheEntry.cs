using System;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Cache.Entry
{
	/// <summary>
	/// A cached instance of a persistent class
	/// </summary>
	[Serializable]
	public sealed partial class CacheEntry
	{
		private readonly object[] disassembledState;
		private readonly string subclass;
		private readonly bool lazyPropertiesAreUnfetched;
		private readonly object version;


		public CacheEntry(object[] state, IEntityPersister persister, bool unfetched, object version, ISessionImplementor session, object owner)
		{
			//disassembled state gets put in a new array (we write to cache by value!)
			disassembledState = TypeHelper.Disassemble(state, persister.PropertyTypes, null, session, owner);
			subclass = persister.EntityName;
			lazyPropertiesAreUnfetched = unfetched || !persister.IsLazyPropertiesCacheable;
			this.version = version;
		}

		internal CacheEntry(object[] state, string subclass, bool unfetched, object version)
		{
			disassembledState = state;
			this.subclass = subclass;
			lazyPropertiesAreUnfetched = unfetched;
			this.version = version;
		}

		public object Version
		{
			get{return version;}
		}

		public string Subclass
		{
			get { return subclass; }
		}

		public bool AreLazyPropertiesUnfetched
		{
			get { return lazyPropertiesAreUnfetched; }
		}

		public object[] DisassembledState
		{
			get
			{
				// todo: this was added to support initializing an entity's EntityEntry snapshot during reattach;
				// this should be refactored to instead expose a method to assemble a EntityEntry based on this
				// state for return.
				return disassembledState;
			}
		}

		public object[] Assemble(object instance, object id, IEntityPersister persister, IInterceptor interceptor,
		                         ISessionImplementor session)
		{
			if (!persister.EntityName.Equals(subclass))
			{
				throw new AssertionFailure("Tried to assemble a different subclass instance");
			}

			return Assemble(disassembledState, instance, id, persister, interceptor, session);
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