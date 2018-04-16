using System.Collections;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of merge events generated from a session.
	/// </summary>
	public partial interface IMergeEventListener
	{
		/// <summary> Handle the given merge event. </summary>
		/// <param name="event">The merge event to be handled. </param>
		void OnMerge(MergeEvent @event);

		/// <summary> Handle the given merge event. </summary>
		/// <param name="event">The merge event to be handled. </param>
		/// <param name="copiedAlready"></param>
		void OnMerge(MergeEvent @event, IDictionary copiedAlready);
	}
}