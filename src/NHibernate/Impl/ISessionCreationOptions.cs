using System.Data.Common;
using NHibernate.MultiTenancy;

namespace NHibernate.Impl
{
	public interface ISessionCreationOptionsWithMultiTenancy
	{
		//TODO 6.0: Merge to ISessionCreationOptions without setter
		TenantConfiguration TenantConfiguration { get; set; }
	}

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

		bool ShouldAutoJoinTransaction { get; }

		DbConnection UserSuppliedConnection { get; }

		IInterceptor SessionInterceptor { get; }

		// Todo: port PhysicalConnectionHandlingMode
		ConnectionReleaseMode SessionConnectionReleaseMode { get; }
	}
}
