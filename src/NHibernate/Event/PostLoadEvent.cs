using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Occurs after an an entity instance is fully loaded.
	/// </summary>
	[Serializable]
	public class PostLoadEvent : AbstractEvent, IPostDatabaseOperationEventArgs
	{
		private object entity;
		private object id;
		private IEntityPersister persister;

		public PostLoadEvent(IEventSource source) : base(source) { }

		public object Entity
		{
			get { return entity; }
			set { entity = value; }
		}

		public object Id
		{
			get { return id; }
			set { id = value; }
		}

		public IEntityPersister Persister
		{
			get { return persister; }
			set { persister = value; }
		}
	}
}
