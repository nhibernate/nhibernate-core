using System;
using NHibernate.Engine;

namespace NHibernate.Event
{
	[Serializable]
	public class FlushEntityEvent : AbstractEvent
	{
		private readonly object entity;
		private readonly EntityEntry entityEntry;

		private object[] propertyValues;
		private object[] databaseSnapshot;
		private int[] dirtyProperties;
		private bool hasDirtyCollection;
		private bool dirtyCheckPossible;
		private bool dirtyCheckHandledByInterceptor;

		public FlushEntityEvent(IEventSource source, object entity, EntityEntry entry)
			: base(source)
		{
			this.entity = entity;
			entityEntry = entry;
		}

		public object Entity
		{
			get { return entity; }
		}

		public EntityEntry EntityEntry
		{
			get { return entityEntry; }
		}

		public object[] PropertyValues
		{
			get { return propertyValues; }
			set { propertyValues = value; }
		}

		public object[] DatabaseSnapshot
		{
			get { return databaseSnapshot; }
			set { databaseSnapshot = value; }
		}

		public int[] DirtyProperties
		{
			get { return dirtyProperties; }
			set { dirtyProperties = value; }
		}

		public bool HasDirtyCollection
		{
			get { return hasDirtyCollection; }
			set { hasDirtyCollection = value; }
		}

		public bool DirtyCheckPossible
		{
			get { return dirtyCheckPossible; }
			set { dirtyCheckPossible = value; }
		}

		public bool DirtyCheckHandledByInterceptor
		{
			get { return dirtyCheckHandledByInterceptor; }
			set { dirtyCheckHandledByInterceptor = value; }
		}

		public bool HasDatabaseSnapshot
		{
			get { return databaseSnapshot != null; }
		}
	}
}