namespace NHibernate.Proxy
{
	/// <summary> 
	/// Delegate to handle the scenario of an entity not found by a specified id. 
	/// </summary>
	public interface IEntityNotFoundDelegate
	{
		/// <summary>
		/// Delegate method to handle the scenario of an entity not found.
		/// </summary>
		/// <param name="entityName">The entityName (may be the class fullname)</param>
		/// <param name="id">The requested id not founded.</param>
		void HandleEntityNotFound(string entityName, object id);
	}
}
