using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate.Tool.hbm2ddl
{

	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on a provided
	/// <seealso cref="IConnectionProvider"/>.  Essentially, ensures that the connection
	/// gets cleaned up, but that the provider itself remains usable since it
	/// was externally provided to us.
	/// </summary>
	public class SuppliedConnectionProviderConnectionHelper : IConnectionHelper
	{
		private readonly IConnectionProvider provider;
		private DbConnection connection;

		public SuppliedConnectionProviderConnectionHelper(IConnectionProvider provider)
		{
			this.provider = provider;
		}

		public void Prepare()
		{
			connection = (DbConnection)provider.GetConnection();
		}

		public DbConnection Connection
		{
			get { return connection; }
		}

		public void Release()
		{
			if (connection != null)
			{
				provider.CloseConnection(connection);
			}
		}
	}
}
