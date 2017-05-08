using System.Data.Common;

namespace NHibernate.Impl
{
	/// <summary>
	/// Options for session creation.
	/// </summary>
	/// <seealso cref="ISessionBuilder"/>
	public interface ISessionCreationOptions
	{
		// NH note: naming "adjusted" for converting Java methods to properties while avoiding conflicts with
		// ISessionBuilder.
		FlushMode InitialSessionFlushMode { get; }

		bool ShouldAutoClose { get; }

		DbConnection UserSuppliedConnection { get; }

		IInterceptor SessionInterceptor { get; }

		// Todo: port PhysicalConnectionHandlingMode
		ConnectionReleaseMode SessionConnectionReleaseMode { get; }
	}
}