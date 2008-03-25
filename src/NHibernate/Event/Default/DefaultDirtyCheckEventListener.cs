using System;
using log4net;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default dirty-check event listener used by hibernate for
	/// checking the session for dirtiness in response to generated dirty-check events. 
	/// </summary>
	[Serializable]
	public class DefaultDirtyCheckEventListener : AbstractFlushingEventListener, IDirtyCheckEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultDirtyCheckEventListener));

		public void OnDirtyCheck(DirtyCheckEvent @event)
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
