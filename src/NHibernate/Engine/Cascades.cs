using System;

namespace NHibernate.Engine {

	/// <summary>
	/// The types of children to cascade to
	/// </summary>
	public enum CascadePoint {
		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent
		/// entity and just before deletion
		/// </summary>
		CascadeAfterInsertBeforeDelete = 1,
		/// <summary>
		/// A cascade point that occurs just before the insertion of the parent entity
		/// and just after deletion
		/// </summary>
		CascadeBeforeInsertAfterDelete = 2,
		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent entity
		/// and just before deletion, inside a collection
		/// </summary>
		CascadeAfterInsertBeforeDeleteViaCollection = 3,
		/// <summary>
		/// A cascade point that occurs just after the update of the parent entity
		/// </summary>
		CascadeOnUpdate = 0
	}

	/// <summary>
	/// Summary description for Cascades.
	/// </summary>
	public class Cascades
	{
		public Cascades()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
