using System;
using System.Collections;
using System.Data;
using System.Reflection;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Test 
{
	public abstract class TestCase
	{
		private const bool OUTPUT_DDL = false;
		protected Configuration cfg;
		protected Dialect.Dialect dialect;
		protected ISessionFactory sessions;
		private ISession lastOpenedSession;

		/// <summary>
		/// Mapping files used in the TestCase
		/// </summary>
		protected abstract IList Mappings { get; }

		/// <summary>
		/// Assembly to load mapping files from (default is NHibernate.DomainModel).
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "NHibernate.DomainModel"; }
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ExportSchema(Mappings);
		}

		/// <summary>
		/// Removes the tables used in this TestCase.
		/// </summary>
		/// <remarks>
		/// If the tables are not cleaned up sometimes SchemaExport runs into
		/// Sql errors because it can't drop tables because of the FKs.  This 
		/// will occur if the TestCase does not have the same hbm.xml files
		/// included as a previous one.
		/// </remarks>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			DropSchema();
		}

		protected virtual void OnSetUp()
		{
		}

		[SetUp]
		public void SetUp()
		{
			OnSetUp();
		}

		protected virtual void OnTearDown()
		{
		}

		/// <summary>
		/// Checks that the test case cleans up after itself. This method
		/// is not overridable, but it calls <see cref="OnTearDown" /> which is.
		/// </summary>
		[TearDown]
		public void TearDown() 
		{
			OnTearDown();
			CheckSessionIsClosed();
			CheckDatabaseIsClean();
		}

		private void CheckSessionIsClosed()
		{
			if( lastOpenedSession != null && lastOpenedSession.IsOpen )
			{
				lastOpenedSession.Close();
				Assert.Fail("Unclosed session");
			}
		}

		private void CheckDatabaseIsClean()
		{
			using( ISession s = sessions.OpenSession() )
			{
				int count = s.Delete( "from System.Object o" );
				s.Flush();
				Assert.AreEqual( 0, count, "Test didn't clean up the database after itself" );
			}
		}

		public void ExportSchema(IList files) 
		{
			ExportSchema(files, true);
		}

		public void ExportSchema(IList files, bool exportSchema) 
		{
			ExportSchema( files, exportSchema, MappingsAssembly );
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

		protected ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}
	}
}
