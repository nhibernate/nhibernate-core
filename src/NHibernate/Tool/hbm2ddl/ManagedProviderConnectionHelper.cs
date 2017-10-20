using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on an internally 
	/// built and managed <seealso cref="ConnectionProvider"/>.
	/// </summary>
	public partial class ManagedProviderConnectionHelper : IConnectionHelper
	{
		private readonly IDictionary<string, string> cfgProperties;
		private IConnectionProvider connectionProvider;
		private DbConnection connection;

		public ManagedProviderConnectionHelper(IDictionary<string, string> cfgProperties)
		{
			this.cfgProperties = cfgProperties;
		}

		public void Prepare()
		{
			connectionProvider = ConnectionProviderFactory.NewConnectionProvider(cfgProperties);
			connection = connectionProvider.GetConnection();
		}

		public DbConnection Connection
		{
			get { return connection; }
		}

		public void Release()
		{
			if (connection != null)
			{
				connectionProvider.CloseConnection(connection);
			}
			connection = null;
		}
	}
}
