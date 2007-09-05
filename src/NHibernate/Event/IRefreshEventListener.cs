using System.Collections;

namespace NHibernate.Event
{
	/// <summary>
	/// Defines the contract for handling of refresh events generated from a session.
	/// </summary>
	public interface IRefreshEventListener
	{
		/// <summary> Handle the given refresh event. </summary>
		/// <param name="theEvent">The refresh event to be handled.</param>
		void OnRefresh(RefreshEvent theEvent);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="theEvent"></param>
		/// <param name="refreshedAlready"></param>
		void OnRefresh(RefreshEvent theEvent, IDictionary refreshedAlready);
	}
}