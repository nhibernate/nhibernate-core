namespace NHibernate.Tool.hbm2ddl
{
	using System.Data;
	using Connection;

	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on a provided
	/// <seealso cref="IConnectionProvider"/>.  Essentially, ensures that the connection
	/// gets cleaned up, but that the provider itself remains usable since it
	/// was externally provided to us.
	/// </summary>
	public class SuppliedConnectionProviderConnectionHelper : IConnectionHelper
	{
		private readonly IConnectionProvider provider;
		private IDbConnection connection;

		public SuppliedConnectionProviderConnectionHelper(IConnectionProvider provider)
		{
			this.provider = provider;
		}

		public void Prepare()
		{
			connection = provider.GetConnection();
		}

		public IDbConnection GetConnection()
		{
			return connection;
		}

		public void Release()
		{
			if(connection!=null)
			{
				provider.CloseConnection(connection);
			}
		}
	}
}
