namespace NHibernate.Tool.hbm2ddl
{
	using System.Data;

	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on an explicitly supplied
	/// connection.
	/// </summary>
	public class SuppliedConnectionHelper : IConnectionHelper
	{
		private IDbConnection connection;

		public SuppliedConnectionHelper(IDbConnection connection)
		{
			this.connection = connection;
		}

		public void Prepare()
		{
		}

		public IDbConnection GetConnection()
		{
			return connection;
		}

		public void Release()
		{
			connection = null;
		}
	}
}
