using NHibernate.AdoNet;

namespace NHibernate.Impl
{
	/// <summary>
	/// An extension of SessionCreationOptions for cases where the Session to be created shares
	/// some part of the "transaction context" of another Session.
	/// </summary>
	/// <seealso cref="ISharedSessionBuilder"/>
	public interface ISharedSessionCreationOptions : ISessionCreationOptions
	{
		// NH note: naming "adjusted" for converting Java methods to properties while avoiding conflicts with
		// ISessionBuilder.
		bool IsTransactionCoordinatorShared { get; }
		// NH different implementation: need to port Hibernate transaction management.
		ConnectionManager ConnectionManager { get; }
	}
}