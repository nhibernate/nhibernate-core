using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

using NHibernate.DomainModel;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Test 
{
	
	public abstract class TestCase 
	{
		protected ISessionFactory sessions;
		protected Dialect.Dialect dialect;

		public void ExportSchema(string[] files) 
		{
			ExportSchema(files, true);
		}

		public void ExportSchema(string[] files, bool exportSchema) 
		{
			Configuration cfg = new Configuration();

			for (int i=0; i<files.Length; i++) 
			{
				cfg.AddResource("NHibernate.DomainModel." + files[i], Assembly.Load("NHibernate.DomainModel"));
			}

			dialect = Dialect.Dialect.GetDialect();

			if(exportSchema) new SchemaExport(cfg).Create(true, true);

			sessions = cfg.BuildSessionFactory( );
		}

		public void ExecuteStatement(string sql)
		{
			ExecuteStatement(sql, true);
		}

		public void ExecuteStatement(string sql, bool error)
		{
			SqlConnection conn = null;
			SqlTransaction tran = null;
			try
			{
				conn = new SqlConnection("Server=localhost;initial catalog=nhibernate;User ID=someuser;Password=somepwd");
				conn.Open();
				tran = conn.BeginTransaction();
				System.Data.SqlClient.SqlCommand comm = conn.CreateCommand();
				comm.CommandText = sql;
				comm.Transaction = tran;
				comm.CommandType = CommandType.Text;
				comm.ExecuteNonQuery();
				tran.Commit();
			}
			catch
			{
				tran.Rollback();
				if (error)
					throw;
			}
			finally
			{
				conn.Close();
			}
		}
	}
}
