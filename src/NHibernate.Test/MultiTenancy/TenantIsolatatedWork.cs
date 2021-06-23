using System.Data.Common;
using Microsoft.Data.SqlClient;
using NHibernate.Engine.Transaction;

namespace NHibernate.Test.MultiTenancy
{
	public partial class TenantIsolatatedWork : IIsolatedWork
	{
		private readonly string _tenantName;

		public TenantIsolatatedWork(string tenantName)
		{
			_tenantName = tenantName;
		}

		public void DoWork(DbConnection connection, DbTransaction transaction)
		{
			var con = (SqlConnection) connection;
			var builder = new SqlConnectionStringBuilder(con.ConnectionString);
			if (builder.ApplicationName != _tenantName)
				throw new HibernateException("Invalid tenant connection");
		}
	}
}
