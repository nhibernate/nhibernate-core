using System;

namespace NHibernate.Event
{
	/// <summary>Defines an event class for the deletion of an entity. </summary>
	[Serializable]
	public class DeleteEvent : AbstractEvent
	{
		private readonly string entityName;
		private readonly object entity;
		private readonly bool cascadeDeleteEnabled;

		/// <summary> Constructs a new DeleteEvent instance. </summary>
		/// <param name="entity">The entity to be deleted.</param>
		/// <param name="source">The session from which the delete event was generated.
		/// </param>
		public DeleteEvent(object entity, IEventSource source)
			: base(source)
		{
			if (entity == null)
				throw new ArgumentNullException("entity", "Attempt to create delete event with null entity");

			this.entity = entity;
		}

		public DeleteEvent(string entityName, object entity, IEventSource source)
			: this(entity, source)
		{
			this.entityName = entityName;
		}

		public DeleteEvent(string entityName, object entity, bool isCascadeDeleteEnabled, IEventSource source)
			: this(entityName, entity, source)
		{
			cascadeDeleteEnabled = isCascadeDeleteEnabled;
		}

		public string EntityName
		{
			get { return entityName; }
		}

		/// <summary>
		/// Returns the encapsulated entity to be deleed.
		/// </summary>
		public object Entity
		{
			get { return entity; }
		}

		public bool CascadeDeleteEnabled
		{
			get { return cascadeDeleteEnabled; }
		}
	}
}