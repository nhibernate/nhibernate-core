using System;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default dirty-check event listener used by hibernate for
	/// checking the session for dirtiness in response to generated dirty-check events. 
	/// </summary>
	[Serializable]
	public partial class DefaultDirtyCheckEventListener : AbstractFlushingEventListener, IDirtyCheckEventListener
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(DefaultDirtyCheckEventListener));

		protected override object Anything => IdentityMap.Instantiate(10);

		protected override CascadingAction CascadingAction => CascadingAction.Persist;

		public virtual void OnDirtyCheck(DirtyCheckEvent @event)
		{
			int oldSize = @event.Session.ActionQueue.CollectionRemovalsCount;

			try
			{
				FlushEverythingToExecutions(@event);
				bool wasNeeded = @event.Session.ActionQueue.HasAnyQueuedActions;
				log.Debug(wasNeeded ? "session dirty" : "session not dirty");
				@event.Dirty = wasNeeded;
			}
			finally
			{
				@event.Session.ActionQueue.ClearFromFlushNeededCheck(oldSize);
			}
		}
	}
}
