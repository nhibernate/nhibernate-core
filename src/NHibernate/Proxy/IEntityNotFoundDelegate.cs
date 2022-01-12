namespace NHibernate.Proxy
{
	//6.0 TODO Add to IEntityNotFoundDelegate interface
	internal static class EntityNotFoundDelegateExtension
	{
		/// <summary>
		/// Method to handle the scenario of an entity not found by unique key.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <param name="entityName">The entityName (may be the class fullname)</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="key">Key</param>
		public static void HandleEntityNotFound(
			this IEntityNotFoundDelegate interceptor,
			string entityName,
			string propertyName,
			object key)
		{
			if (interceptor is IEntityNotFoundByUniqueKeyDelegate x)
			{
				x.HandleEntityNotFound(entityName, propertyName, key);
				return;
			}

			new Impl.SessionFactoryImpl.DefaultEntityNotFoundDelegate().HandleEntityNotFound(entityName, propertyName, key);
		}
	}

	/// <summary> 
	/// Delegate to handle the scenario of an entity not found by a specified id and property.
	/// This is a temporary interface until it can be merged into IEntityNotFoundDelegate 
	/// </summary>
	//6.0 TODO Remove and add to method to IEntityNotFoundDelegate interface
	public interface IEntityNotFoundByUniqueKeyDelegate
	{
		void HandleEntityNotFound(string entityName, string propertyName, object key);
	}
	
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
