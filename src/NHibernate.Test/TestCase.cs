using System;
using System.Collections;
using System.Data;
using System.Reflection;

using NHibernate.DomainModel;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Test 
{
	
	public abstract class TestCase 
	{
		private const bool OUTPUT_DDL = true;
		protected Configuration cfg;
		protected Dialect.Dialect dialect;
		protected ISessionFactory sessions;

		/// <summary>
		/// Removes the tables used in this TestCase.
		/// </summary>
		/// <remarks>
		/// If the tables are not cleaned up sometimes SchemaExport runs into
		/// Sql errors because it can't drop tables because of the FKs.  This 
		/// will occur if the TestCase does not have the same hbm.xml files
		/// included as a previous one.
		/// </remarks>
		[TearDown]
		public virtual void TearDown() 
		{
			DropSchema();
		}

		public void ExportSchema(IList files) 
		{
			ExportSchema(files, true);
		}

		public void ExportSchema(IList files, bool exportSchema) 
		{
			ExportSchema( files, exportSchema, "NHibernate.DomainModel" );
		}

		public void ExportSchema(IList files, bool exportSchema, string assemblyName) 
		{
			cfg = new Configuration();

			for (int i=0; i<files.Count; i++) 
			{
				cfg.AddResource( assemblyName + "." + files[i].ToString(), Assembly.Load( assemblyName ) );
			}

			if(exportSchema) new SchemaExport(cfg).Create(OUTPUT_DDL, true);
			
			sessions = cfg.BuildSessionFactory( );
			dialect = Dialect.Dialect.GetDialect();
		}

		/// <summary>
		/// Drops the schema that was built with the TestCase's Configuration.
		/// </summary>
		public void DropSchema() 
		{
			new SchemaExport(cfg).Drop(OUTPUT_DDL, true);
		}

		public void ExecuteStatement(string sql)
		{
			ExecuteStatement(sql, true);
		}

		public void ExecuteStatement(string sql, bool error)
		{
			IDbConnection conn = null;
			IDbTransaction tran = null;
			try
			{
				if (cfg == null)
					cfg = new Configuration();
				Connection.IConnectionProvider prov = Connection.ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);
				conn = prov.GetConnection();
				tran = conn.BeginTransaction();
				IDbCommand comm = conn.CreateCommand();
				comm.CommandText = sql;
				comm.Transaction = tran;
				comm.CommandType = CommandType.Text;
				comm.ExecuteNonQuery();
				tran.Commit();
			}
			catch(Exception)
			{
				if (tran != null)
					tran.Rollback();
				if (error)
					throw;
			}
			finally
			{
				if (conn != null)
					conn.Close();
			}
		}

	}
}
