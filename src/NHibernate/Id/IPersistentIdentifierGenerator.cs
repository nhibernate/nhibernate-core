using System;

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
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// An array of <see cref="String"/> objects that contain the sql to create the 
		/// necessary database objects.
		/// </returns>
		string[] SqlCreateStrings(Dialect.Dialect dialect);

		/// <summary>
		/// The SQL required to remove the underlying database objects
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// A <see cref="String"/> that will drop the database objects.
		/// </returns>
		string SqlDropString(Dialect.Dialect dialect);

		/// <summary>
		/// Return a key unique to the underlying database objects.
		/// </summary>
		/// <returns>
		/// A key unique to the underlying database objects.
		/// </returns>
		/// <remarks>
		/// Prevents us from trying to create/remove them multiple times
		/// </remarks>
		object GeneratorKey();
	}
}