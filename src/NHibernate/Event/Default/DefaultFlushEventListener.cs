using System;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default flush event listeners used by hibernate for 
	/// flushing session state in response to generated flush events. 
	/// </summary>
	[Serializable]
	public partial class DefaultFlushEventListener : AbstractFlushingEventListener, IFlushEventListener
	{
		public virtual void OnFlush(FlushEvent @event)
		{
			IEventSource source = @event.Session;

			if ((source.PersistenceContext.EntityEntries.Count > 0) || (source.PersistenceContext.CollectionEntries.Count > 0))
			{
				FlushEverythingToExecutions(@event);
				PerformExecutions(source);
				PostFlush(source);

				if (source.Factory.Statistics.IsStatisticsEnabled)
				{
					source.Factory.StatisticsImplementor.Flush();
				}
			}
		}
	}
}
