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
using System.Collections.Concurrent;
using System.IO;
using NUnit.Framework.Interfaces;
using System.Text;
using log4net.Util;
using static NUnit.Framework.TestContext;

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

		private ConcurrentBag<ISession> _openedSessions = new ConcurrentBag<ISession>();
		private ISession _lastOpenedSession;
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
			XmlConfigurator.Configure(LogManager.GetRepository(typeof(TestCase).Assembly));
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[OneTimeSetUp]
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
		[OneTimeTearDown]
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
			var testResult = TestContext.CurrentContext.Result;
			var fail = false;
			string badCleanupMessage = null;
			try
			{
				OnTearDown();
				var wereClosed = CheckSessionsWereClosed();
				var wasCleaned = CheckDatabaseWasCleaned();
				var wereConnectionsClosed = CheckConnectionsWereClosed();
				fail = !wereClosed || !wasCleaned || !wereConnectionsClosed;

				if (fail)
				{
					badCleanupMessage = "Test didn't clean up after itself. session closed: " + wereClosed + "; database cleaned: " + wasCleaned
						+ "; connection closed: " + wereConnectionsClosed;
					if (testResult != null && testResult.Outcome.Status == TestStatus.Failed)
					{
						// Avoid hiding a test failure (asserts are usually not hidden, but other exception would be).
						badCleanupMessage = GetCombinedFailureMessage(testResult, badCleanupMessage, null);
					}
				}
			}
			catch (Exception ex)
			{
				if (testResult == null || testResult.Outcome.Status != TestStatus.Failed)
					throw;

				// Avoid hiding a test failure (asserts are usually not hidden, but other exceptions would be).
				var exType = ex.GetType();
				Assert.Fail(GetCombinedFailureMessage(testResult,
					exType.Namespace + "." + exType.Name + " " + ex.Message,
					ex.StackTrace));
			}

			if (fail)
			{
				Assert.Fail(badCleanupMessage);
			}
		}

		private string GetCombinedFailureMessage(ResultAdapter result, string tearDownFailure, string tearDownStackTrace)
		{
			var message = new StringBuilder()
				.Append("The test failed and then failed to cleanup. Test failure is: ")
				.AppendLine(result.Message)
				.Append("Tear-down failure is: ")
				.AppendLine(tearDownFailure)
				.AppendLine("Test failure stack trace is: ")
				.AppendLine(result.StackTrace);

			if (!string.IsNullOrEmpty(tearDownStackTrace))
				message.AppendLine("Tear-down failure stack trace is:")
					.Append(tearDownStackTrace);

			return message.ToString();
		}

		// Factory is exposed to descendant, any session opened directly from it will not be handled by following check.
		private bool CheckSessionsWereClosed()
		{
			var allClosed = true;
			foreach (var session in _openedSessions)
			{
				if (session.IsOpen)
				{
					log.Error($"Test case didn't close session {session.GetSessionImplementation().SessionId}, closing");
					allClosed = false;
					session.Close();
				}
			}

			return allClosed;
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
			_lastOpenedSession = null;
			_openedSessions = new ConcurrentBag<ISession>();
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
				var conn = prov.GetConnection();

				try
				{
					using (var tran = conn.BeginTransaction())
					using (var comm = conn.CreateCommand())
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
			using (var cmd = session.Connection.CreateCommand())
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
			_lastOpenedSession = sessions.OpenSession();
			_openedSessions.Add(_lastOpenedSession);
			return _lastOpenedSession;
		}

		protected virtual ISession OpenSession(IInterceptor sessionLocalInterceptor)
		{
			_lastOpenedSession = sessions.WithOptions().Interceptor(sessionLocalInterceptor).OpenSession();
			_openedSessions.Add(_lastOpenedSession);
			return _lastOpenedSession;
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
