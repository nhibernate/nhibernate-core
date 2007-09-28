using System;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// A cached instance of a persistent class
	/// </summary>
	[Serializable]
	public sealed class CacheEntry
	{
		private readonly object[] state;
		private readonly System.Type subclass;

		/// <summary></summary>
		public System.Type Subclass
		{
			get { return subclass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		public CacheEntry(object obj, IEntityPersister persister, ISessionImplementor session)
		{
			state = Disassemble(obj, persister, session);
			subclass = obj.GetType();
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
			if (subclass != persister.MappedClass)
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