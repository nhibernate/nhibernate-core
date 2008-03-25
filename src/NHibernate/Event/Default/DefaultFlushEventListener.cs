using System;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default flush event listeners used by hibernate for 
	/// flushing session state in response to generated flush events. 
	/// </summary>
	[Serializable]
	public class DefaultFlushEventListener : AbstractFlushingEventListener, IFlushEventListener
	{
		public void OnFlush(FlushEvent @event)
		{
			IEventSource source = @event.Session;

			if (source.PersistenceContext.HasNonReadOnlyEntities)
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
