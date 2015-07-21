using System;
using System.Collections;
using System.Data;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using NUnit.Framework;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Test
{
	public abstract class TestCase
	{
		private const bool OutputDdl = false;
		protected Configuration cfg;
		protected ISessionFactoryImplementor sessions;

		private static readonly ILog log = LogManager.GetLogger(typeof(TestCase));

		protected Dialect.Dialect Dialect
		{
			get { return NHibernate.Dialect.Dialect.GetDialect(cfg.Properties); }
		}

		protected TestDialect TestDialect
		{
			get { return TestDialect.GetTestDialect(Dialect); }
		}

		/// <summary>
		/// To use in in-line test
		/// </summary>
		protected bool IsAntlrParser
		{
			get
			{
				return sessions.Settings.QueryTranslatorFactory is ASTQueryTranslatorFactory;
			}
		}

		protected ISession lastOpenedSession;
		private DebugConnectionProvider connectionProvider;

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

		static TestCase()
		{
			// Configure log4net here since configuration through an attribute doesn't always work.
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			try
			{
				Configure();
				if (!AppliesTo(Dialect))
				{
					Assert.Ignore(GetType() + " does not apply to " + Dialect);
				}

				CreateSchema();
				try
				{
					BuildSessionFactory();
					if (!AppliesTo(sessions))
					{
						Assert.Ignore(GetType() + " does not apply with the current session-factory configuration");
					}
				}
				catch
				{
					DropSchema();
					throw;
				}
			}
			catch (Exception e)
			{
				Cleanup();
				log.Error("Error while setting up the test fixture", e);
				throw;
			}
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
			// If TestFixtureSetup fails due to an IgnoreException, it will still run the teardown.
			// We don't want to try to clean up again since the setup would have already done so.
			// If cfg is null already, that indicates it's already been cleaned up and we needn't.
			if (cfg != null)
			{
				if (!AppliesTo(Dialect))
					return;

				DropSchema();
				Cleanup();
			}
		}

		protected virtual void OnSetUp()
		{
		}

		/// <summary>
		/// Set up the test. This method is not overridable, but it calls
		/// <see cref="OnSetUp" /> which is.
		/// </summary>
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

			bool wasClosed = CheckSessionWasClosed();
			bool wasCleaned = CheckDatabaseWasCleaned();
			bool wereConnectionsClosed = CheckConnectionsWereClosed();
			bool fail = !wasClosed || !wasCleaned || !wereConnectionsClosed;

			if (fail)
			{
				Assert.Fail("Test didn't clean up after itself. session closed: " + wasClosed + " database cleaned: "+ wasCleaned
					+ " connection closed: " + wereConnectionsClosed);
			}
		}

		private bool CheckSessionWasClosed()
		{
			if (lastOpenedSession != null && lastOpenedSession.IsOpen)
			{
				log.Error("Test case didn't close a session, closing");
				lastOpenedSession.Close();
				return false;
			}

			return true;
		}

		protected virtual bool CheckDatabaseWasCleaned()
		{
			if (sessions.GetAllClassMetadata().Count == 0)
			{
				// Return early in the case of no mappings, also avoiding
				// a warning when executing the HQL below.
				return true;
			}

			bool empty;
			using (ISession s = sessions.OpenSession())
			{
				IList objects = s.CreateQuery("from System.Object o").List();
				empty = objects.Count == 0;
			}

			if (!empty)
			{
				log.Error("Test case didn't clean up the database after itself, re-creating the schema");
				DropSchema();
				CreateSchema();
			}

			return empty;
		}

		private bool CheckConnectionsWereClosed()
		{
			if (connectionProvider == null || !connectionProvider.HasOpenConnections)
			{
				return true;
			}

			log.Error("Test case didn't close all open connections, closing");
			connectionProvider.CloseAllConnections();
			return false;
		}

		private void Configure()
		{
			cfg = TestConfigurationHelper.GetDefaultConfiguration();

			AddMappings(cfg);

			Configure(cfg);

			ApplyCacheSettings(cfg);
		}

		protected virtual void AddMappings(Configuration configuration)
		{
			Assembly assembly = Assembly.Load(MappingsAssembly);

			foreach (string file in Mappings)
			{
				configuration.AddResource(MappingsAssembly + "." + file, assembly);
			}
		}

		protected virtual void CreateSchema()
		{
			new SchemaExport(cfg).Create(OutputDdl, true);
		}

		protected virtual void DropSchema()
		{
			new SchemaExport(cfg).Drop(OutputDdl, true);
		}

		protected virtual void BuildSessionFactory()
		{
			sessions = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			connectionProvider = sessions.ConnectionProvider as DebugConnectionProvider;
		}

		private void Cleanup()
		{
			if (sessions != null)
			{
				sessions.Close();
			}
			sessions = null;
			connectionProvider = null;
			lastOpenedSession = null;
			cfg = null;
		}

		public int ExecuteStatement(string sql)
		{
			if (cfg == null)
			{
				cfg = TestConfigurationHelper.GetDefaultConfiguration();
			}

			using (IConnectionProvider prov = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties))
			{
				IDbConnection conn = prov.GetConnection();

				try
				{
					using (IDbTransaction tran = conn.BeginTransaction())
					using (IDbCommand comm = conn.CreateCommand())
					{
						comm.CommandText = sql;
						comm.Transaction = tran;
						comm.CommandType = CommandType.Text;
						int result = comm.ExecuteNonQuery();
						tran.Commit();
						return result;
					}
				}
				finally
				{
					prov.CloseConnection(conn);
				}
			}
		}

		public int ExecuteStatement(ISession session, ITransaction transaction, string sql)
		{
			using (IDbCommand cmd = session.Connection.CreateCommand())
			{
				cmd.CommandText = sql;
				if (transaction != null)
					transaction.Enlist(cmd);
				return cmd.ExecuteNonQuery();
			}
		}

		protected ISessionFactoryImplementor Sfi
		{
			get { return sessions; }
		}

		protected virtual ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}

		protected virtual ISession OpenSession(IInterceptor sessionLocalInterceptor)
		{
			lastOpenedSession = sessions.OpenSession(sessionLocalInterceptor);
			return lastOpenedSession;
		}

		protected virtual void ApplyCacheSettings(Configuration configuration)
		{
			if (CacheConcurrencyStrategy == null)
			{
				return;
			}

			foreach (PersistentClass clazz in configuration.ClassMappings)
			{
				bool hasLob = false;
				foreach (Property prop in clazz.PropertyClosureIterator)
				{
					if (prop.Value.IsSimpleValue)
					{
						IType type = ((SimpleValue)prop.Value).Type;
						if (type == NHibernateUtil.BinaryBlob)
						{
							hasLob = true;
						}
					}
				}
				if (!hasLob && !clazz.IsInherited)
				{
					configuration.SetCacheConcurrencyStrategy(clazz.EntityName, CacheConcurrencyStrategy);
				}
			}

			foreach (Mapping.Collection coll in configuration.CollectionMappings)
			{
				configuration.SetCollectionCacheConcurrencyStrategy(coll.Role, CacheConcurrencyStrategy);
			}
		}

		#region Properties overridable by subclasses

		protected virtual bool AppliesTo(Dialect.Dialect dialect)
		{
			return true;
		}

		protected virtual bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return true;
		}

		protected virtual void Configure(Configuration configuration)
		{
		}

		protected virtual string CacheConcurrencyStrategy
		{
			get { return "nonstrict-read-write"; }
			//get { return null; }
		}

		#endregion
	}
}
