using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Represents an operation we performed against the database. 
	/// </summary>
	[Serializable]
	public class AbstractPostDatabaseOperationEvent : AbstractEvent, IPostDatabaseOperationEventArgs
	{
		/// <summary> Constructs an event containing the pertinent information. </summary>
		/// <param name="source">The session from which the event originated. </param>
		/// <param name="entity">The entity to be invloved in the database operation. </param>
		/// <param name="id">The entity id to be invloved in the database operation. </param>
		/// <param name="persister">The entity's persister. </param>
		protected AbstractPostDatabaseOperationEvent(IEventSource source, object entity, object id, IEntityPersister persister)
			: base(source)
		{
			Entity = entity;
			Id = id;
			Persister = persister;
		}

		/// <summary> The entity involved in the database operation. </summary>
		public object Entity { get; private set; }

		/// <summary> The id to be used in the database operation. </summary>
		public object Id { get; private set; }

		/// <summary> 
		/// The persister for the <see cref="Entity"/>. 
		/// </summary>
		public IEntityPersister Persister { get; private set; }
	}
}
