namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that requires creation of database objects
	/// All <see cref="IPersistentIdentifierGenerator"/>s that also implement 
	/// An <see cref="IConfigurable" />  have access to a special mapping parameter: schema
	/// </summary>
	public interface IPersistentIdentifierGenerator : IIdentifierGenerator
	{
		/// <summary>
		/// The SQL required to create the underlying database objects
		/// </summary>
		/// <param name="dialect"></param>
		string[ ] SqlCreateStrings( Dialect.Dialect dialect );

		/// <summary>
		/// The SQL required to remove the underlying database objects
		/// </summary>
		/// <param name="dialect"></param>
		string SqlDropString( Dialect.Dialect dialect );

		/// <summary>
		/// Return a key unique to the underlying database objects.
		/// </summary>
		/// <remarks>
		/// Prevents us from trying to create/remove them multiple times
		/// </remarks>
		object GeneratorKey();
	}
}