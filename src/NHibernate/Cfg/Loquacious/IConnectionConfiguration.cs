using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.Connection;

namespace NHibernate.Cfg.Loquacious
{
	public interface IConnectionConfiguration
	{
		IConnectionConfiguration Trough<TProvider>() where TProvider : IConnectionProvider;
		IConnectionConfiguration Through<TDriver>() where TDriver : IDriver;
		IConnectionConfiguration With(IsolationLevel level);
		IConnectionConfiguration Releasing(ConnectionReleaseMode releaseMode);
		IDbIntegrationConfiguration Using(string connectionString);
		IDbIntegrationConfiguration Using(DbConnectionStringBuilder connectionStringBuilder);
		IDbIntegrationConfiguration ByAppConfing(string connectionStringName);
	}
}