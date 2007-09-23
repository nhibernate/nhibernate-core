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

			// NH: Different behavior
			// in H3.2 they use persist but we don't have the same behavior
			// to maintains backdraw compatibility with NH1.2.x we perform flush any way.
			FlushEverythingToExecutions(@event);
			PerformExecutions(source);
			PostFlush(source);

			//if (source.HasNonReadOnlyEntities)
			//{
			//  FlushEverythingToExecutions(@event);
			//  PerformExecutions(source);
			//  PostFlush(source);

			//  // TODO: H3.2 not ported
			//  //if (source.Factory.Statistics.StatisticsEnabled)
			//  //{
			//  //  source.Factory.StatisticsImplementor.flush();
			//  //}
			//}
		}
	}
}
