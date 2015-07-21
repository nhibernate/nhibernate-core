using System.Data;
using System.Data.Common;
using NHibernate.Driver;
using NHibernate.Connection;

namespace NHibernate.Cfg.Loquacious
{
	public interface IConnectionConfiguration
	{
		IConnectionConfiguration Through<TProvider>() where TProvider : IConnectionProvider;
		IConnectionConfiguration By<TDriver>() where TDriver : IDriver;
		IConnectionConfiguration With(IsolationLevel level);
		IConnectionConfiguration Releasing(ConnectionReleaseMode releaseMode);
		IDbIntegrationConfiguration Using(string connectionString);
		IDbIntegrationConfiguration Using(DbConnectionStringBuilder connectionStringBuilder);
		IDbIntegrationConfiguration ByAppConfing(string connectionStringName);
	}
}