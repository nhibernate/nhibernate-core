using System;
using System.Collections;
using System.Data;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;

using NUnit.Framework;
using System.IO;

namespace NHibernate.Test
{
	public abstract class TestCase
	{
		private const bool OutputDdl = false;
		protected Configuration cfg;
		protected ISessionFactory sessions;

		private static readonly ILog log = LogManager.GetLogger(typeof(TestCase));

		protected Dialect.Dialect Dialect
		{
			get { return NHibernate.Dialect.Dialect.GetDialect(cfg.Properties); }
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
				BuildSessionFactory();
			}
			catch (Exception e)
			{
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
			DropSchema();
			Cleanup();
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
				Assert.Fail("Test didn't clean up after itself");
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

		private bool CheckDatabaseWasCleaned()
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
			cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			Assembly assembly = Assembly.Load(MappingsAssembly);

			foreach (string file in Mappings)
			{
				cfg.AddResource(MappingsAssembly + "." + file, assembly);
			}

			Configure(cfg);

			ApplyCacheSettings(cfg);
		}

		private void CreateSchema()
		{
			new SchemaExport(cfg).Create(OutputDdl, true);
		}

		private void DropSchema()
		{
			new SchemaExport(cfg).Drop(OutputDdl, true);
		}

		protected virtual void BuildSessionFactory()
		{
			sessions = cfg.BuildSessionFactory();
			connectionProvider = sessions.ConnectionProvider as DebugConnectionProvider;
		}

		private void Cleanup()
		{
			sessions.Close();
			sessions = null;
			connectionProvider = null;
			lastOpenedSession = null;
			cfg = null;
		}

		public int ExecuteStatement(string sql)
		{
			if (cfg == null)
			{
				cfg = new Configuration();
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

		protected virtual ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}

		protected void ApplyCacheSettings(Configuration configuration)
		{
			if (CacheConcurrencyStrategy == null)
			{
				return;
			}

			foreach (PersistentClass clazz in configuration.ClassMappings)
			{
				bool hasLob = false;
				foreach (Mapping.Property prop in clazz.PropertyClosureIterator)
				{
					if (prop.Value.IsSimpleValue)
					{
						IType type = ((SimpleValue) prop.Value).Type;
						if (type == NHibernateUtil.BinaryBlob)
						{
							hasLob = true;
						}
					}
				}
				if (!hasLob && !clazz.IsInherited)
				{
					configuration.SetCacheConcurrencyStrategy(clazz.MappedClass, CacheConcurrencyStrategy);
				}
			}

			foreach (Mapping.Collection coll in configuration.CollectionMappings)
			{
				configuration.SetCacheConcurrencyStrategy(coll.Role, CacheConcurrencyStrategy);
			}
		}

		#region Properties overridable by subclasses

		protected virtual bool AppliesTo(Dialect.Dialect dialect)
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