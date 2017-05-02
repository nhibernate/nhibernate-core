using System.Data.Common;

namespace NHibernate.Impl
{
	/// <summary>
	/// Options for session creation.
	/// </summary>
	/// <seealso cref="ISessionBuilder"/>
	public interface ISessionCreationOptions
	{
		// NH note: it is tempting to convert them to properties, but then their names conflict
		// with ISessionBuilder, which is implemented along this interface.
		FlushMode GetInitialSessionFlushMode();

		bool ShouldAutoClose();

		DbConnection GetConnection();

		IInterceptor GetInterceptor();

		// Todo: port PhysicalConnectionHandlingMode
		ConnectionReleaseMode GetConnectionReleaseMode();
	}
}