using System;
using NHibernate.Dialect;

namespace NHibernate.Id {
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that requires creation of database objects
	/// All <c>IPersistentIdentifierGenerator</c>s that also implement 
	/// <c>IConfigurable</c> have access to a special mapping parameter: schema
	/// </summary>
	public interface IPersistentIdentifierGenerator : IIdentifierGenerator {
		
		/// <summary>
		/// The SQL required to create the underlying database objects
		/// </summary>
		string[] SqlCreateStrings(Dialect.Dialect dialect);

		/// <summary>
		/// The SQL required to remove the underlying database objects
		/// </summary>
		string SqlDropString(Dialect.Dialect dialect);

		/// <summary>
		/// Return a key unique to the underlying database objects.
		/// </summary>
		/// <remarks>
		/// Prevents us from trying to create/remove them multiple times
		/// </remarks>
		object GeneratorKey();
	}
}
