namespace NHibernate.Bytecode
{
	/// <summary>
	/// Interface for instanciate all NHibernate objects.
	/// </summary>
	public interface IObjectsFactory
	{
		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <param name="type">The type of object to create.</param>
		/// <returns>A reference to the created object.</returns>
		object CreateInstance(System.Type type);

		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <param name="type">The type of object to create.</param>
		/// <param name="nonPublic">true if a public or nonpublic default constructor can match; false if only a public default constructor can match.</param>
		/// <returns>A reference to the created object.</returns>
		object CreateInstance(System.Type type, bool nonPublic);

		/// <summary>
		/// Creates an instance of the specified type using the constructor 
		/// that best matches the specified parameters.
		/// </summary>
		/// <param name="type">The type of object to create.</param>
		/// <param name="ctorArgs">An array of constructor arguments.</param>
		/// <returns>A reference to the created object.</returns>
		object CreateInstance(System.Type type, params object[] ctorArgs);
	}
}