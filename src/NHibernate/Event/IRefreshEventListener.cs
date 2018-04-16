using System.Collections;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of refresh events generated from a session.
	/// </summary>
	public partial interface IRefreshEventListener
	{
		/// <summary> Handle the given refresh event. </summary>
		/// <param name="event">The refresh event to be handled.</param>
		void OnRefresh(RefreshEvent @event);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		/// <param name="refreshedAlready"></param>
		void OnRefresh(RefreshEvent @event, IDictionary refreshedAlready);
	}
}