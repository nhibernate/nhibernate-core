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
	public sealed class CacheEntry
	{
		private readonly object[] state;
		private readonly System.Type subclassType;
		private readonly object[] disassembledState;
		private readonly string subclass;
		private readonly bool lazyPropertiesAreUnfetched;
		private readonly object version;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		public CacheEntry(object obj, IEntityPersister persister, ISessionImplementor session)
		{
			state = Disassemble(obj, persister, session);
			subclassType = obj.GetType();
		}

		internal CacheEntry(object[] state, string subclass, bool unfetched, object version)
		{
			disassembledState = state;
			this.subclass = subclass;
			lazyPropertiesAreUnfetched = unfetched;
			this.version = version;
		}

		/// <summary></summary>
		public System.Type SubclassType
		{
			get { return subclassType; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private static object[] Disassemble(object obj, IEntityPersister persister, ISessionImplementor session)
		{
			object[] values = persister.GetPropertyValues(obj);
			IType[] propertyTypes = persister.PropertyTypes;
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = propertyTypes[i].Disassemble(values[i], session);
			}
			return values;
		}

		public object[] Assemble(object instance, object id, IEntityPersister persister, IInterceptor interceptor,
		                         ISessionImplementor session)
		{
			if (subclassType != persister.MappedClass)
			{
				throw new AssertionFailure("Tried to assemble a different subclass instance");
			}

			return Assemble(state, instance, id, persister, interceptor, session);
		}

		private static object[] Assemble(object[] values, object result, object id, IEntityPersister persister,
		                                 IInterceptor interceptor, ISessionImplementor session)
		{
			//assembled state gets put in a new array (we read from cache by value!)
			object[] assembledProps = TypeFactory.Assemble(values, persister.PropertyTypes, session, result);
	
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