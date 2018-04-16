using System.Data.Common;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary>
	/// A <seealso cref="IConnectionHelper"/> implementation based on an explicitly supplied
	/// connection.
	/// </summary>
	public partial class SuppliedConnectionHelper : IConnectionHelper
	{
		private DbConnection connection;

		public SuppliedConnectionHelper(DbConnection connection)
		{
			this.connection = connection;
		}

		public void Prepare()
		{
		}

		public DbConnection Connection
		{
			get { return connection; }
		}

		public void Release()
		{
			connection = null;
		}
	}
}
