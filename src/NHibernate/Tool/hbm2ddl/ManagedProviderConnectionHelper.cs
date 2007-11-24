namespace NHibernate.Tool.hbm2ddl
{
	using System.Collections;
	using System.Data;
	using Connection;

	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on an internally
	///  built and managed <seealso cref="ConnectionProvider"/>.
	/// </summary>
	public class ManagedProviderConnectionHelper : IConnectionHelper
	{
		private readonly IDictionary cfgProperties;
		private IConnectionProvider connectionProvider;
		private IDbConnection connection;

		public ManagedProviderConnectionHelper(IDictionary cfgProperties)
		{
			this.cfgProperties = cfgProperties;
		}

		public void Prepare()
		{
			connectionProvider = ConnectionProviderFactory.NewConnectionProvider(cfgProperties);
			connection = connectionProvider.GetConnection();
		}

		public IDbConnection GetConnection()
		{
			return connection;
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
