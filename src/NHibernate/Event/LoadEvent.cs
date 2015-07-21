using System;

namespace NHibernate.Event
{
	/// <summary>Defines an event class for the loading of an entity. </summary>
	[Serializable]
	public class LoadEvent : AbstractEvent
	{
		public static readonly LockMode DefaultLockMode = LockMode.None;
		private object entityId;
		private string entityClassName;
		private object instanceToLoad;
		private LockMode lockMode;
		private readonly bool isAssociationFetch;
		private object result;

		private LoadEvent(object entityId,string entityClassName, object instanceToLoad, 
			LockMode lockMode, bool isAssociationFetch, IEventSource source)
			: base(source)
		{
			if (entityId == null)
				throw new ArgumentNullException("entityId", "id to load is required for loading");

			if (lockMode == LockMode.Write)
				throw new ArgumentOutOfRangeException("lockMode", "Invalid lock mode for loading");

			if (lockMode == null)
			{
				lockMode = DefaultLockMode;
			}

			this.entityId = entityId;
			this.entityClassName = entityClassName;
			this.instanceToLoad = instanceToLoad;
			this.lockMode = lockMode;
			this.isAssociationFetch = isAssociationFetch;
		}

		public LoadEvent(object entityId, object instanceToLoad, IEventSource source)
			: this(entityId, null, instanceToLoad, null, false, source) {}

		public LoadEvent(object entityId, string entityClassName, LockMode lockMode, IEventSource source)
			: this(entityId, entityClassName, null, lockMode, false, source) {}

		public LoadEvent(object entityId, string entityClassName, bool isAssociationFetch, IEventSource source)
			: this(entityId, entityClassName, null, null, isAssociationFetch, source) {}

		public bool IsAssociationFetch
		{
			get { return isAssociationFetch; }
		}

		public object EntityId
		{
			get { return entityId; }
			set
			{
				if (value == null)
					throw new InvalidOperationException("id to load is required for loading");
				 entityId = value;
			}
		}

		public string EntityClassName
		{
			get { return entityClassName; }
			set { entityClassName = value; }
		}

		public object InstanceToLoad
		{
			get { return instanceToLoad; }
			set { instanceToLoad = value; }
		}

		public LockMode LockMode
		{
			get { return lockMode; }
			set
			{
				if (value == LockMode.Write)
					throw new InvalidOperationException("Invalid lock mode for loading");

				if (value == null)
				{
					lockMode = DefaultLockMode;
				}

				lockMode = value;
			}
		}

		public object Result
		{
			get { return result; }
			set { result = value; }
		}
	}
}