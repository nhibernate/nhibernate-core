using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate
{
	// NH different implementation: will not try to support covariant return type for specializations
	// until it is needed.
	/// <summary>
	/// Represents a consolidation of all stateless session creation options into a builder style delegate.
	/// </summary>
	public interface IStatelessSessionBuilder
	{
		/// <summary>
		/// Opens a session with the specified options.
		/// </summary>
		/// <returns>The session.</returns>
		IStatelessSession OpenStatelessSession();

		/// <summary>
		/// Adds a specific connection to the session options.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		IStatelessSessionBuilder Connection(DbConnection connection);

		// NH remark: seems a bit overkill for now. On Hibernate side, they have at least another option: the tenant.
	}
}