using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs before updating the datastore
	/// </summary>
	public class PreUpdateEvent
	{
		private readonly object entity;
		private readonly object id;
		private readonly object[] state;
		private readonly object[] oldState;
		private readonly IEntityPersister persister;
		private readonly ISessionImplementor source;

		public PreUpdateEvent(object entity, object id, object[] state, object[] oldState, 
			IEntityPersister persister, ISessionImplementor source)
		{
			this.entity = entity;
			this.id = id;
			this.state = state;
			this.oldState = oldState;
			this.persister = persister;
			this.source = source;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public object[] State
		{
			get { return state; }
		}

		public object[] OldState
		{
			get { return oldState; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
		}

		public ISessionImplementor Source
		{
			get { return source; }
		}
	}
}
