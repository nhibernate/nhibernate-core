using NHibernate.Connection;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	public class MockConnectionProvider : IMultiTenantConnectionProvider
	{
		private readonly IConnectionAccess _connectionAccess;

		public MockConnectionProvider(string tenantIdentifier, IConnectionAccess connectionAccess)
		{
			_connectionAccess = connectionAccess;
			TenantIdentifier = tenantIdentifier;
		}

		public string TenantIdentifier { get; }
		public IConnectionAccess GetConnectionAccess()
		{
			return _connectionAccess;
		}
	}
}
