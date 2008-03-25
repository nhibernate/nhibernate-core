using System;

namespace NHibernate.Engine
{
	/// <summary>
	/// Represents the status of an entity with respect to 
	/// this session. These statuses are for internal 
	/// book-keeping only and are not intended to represent 
	/// any notion that is visible to the <b>application</b>. 
	/// </summary>
	[Serializable]
	public enum Status
	{
		/// <summary>
		/// The Entity is snapshotted in the Session with the same state as the database
		/// (called Managed in H3).
		/// </summary>
		Loaded,

		/// <summary>
		/// The Entity is in the Session and has been marked for deletion but not
		/// deleted from the database yet.
		/// </summary>
		Deleted,

		/// <summary>
		/// The Entity has been deleted from database.
		/// </summary>
		Gone,

		/// <summary>
		/// The Entity is in the process of being loaded.
		/// </summary>
		Loading,

		/// <summary>
		/// The Entity is in the process of being saved.
		/// </summary>
		Saving,

		/// <summary>
		/// The entity is read-only.
		/// </summary>
		ReadOnly
	}
}