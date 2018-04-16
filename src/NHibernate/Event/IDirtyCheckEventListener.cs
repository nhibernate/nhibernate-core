namespace NHibernate.Event
{
	/// <summary> Defines the contract for handling of session dirty-check events.</summary>
	public partial interface IDirtyCheckEventListener
	{
		/// <summary>Handle the given dirty-check event. </summary>
		/// <param name="event">The dirty-check event to be handled. </param>
		void OnDirtyCheck(DirtyCheckEvent @event);
	}
}