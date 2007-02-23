using System;
using System.Collections;
using System.Reflection;

using log4net;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace Nullables.Tests.NHibernate
{
	public abstract class TestCase
	{
		private const bool OUTPUT_DDL = false;
		protected Configuration cfg;
		protected ISessionFactory sessions;

		private static readonly ILog log =
			LogManager.GetLogger(typeof(TestCase));

		private ISession lastOpenedSession;

		/// <summary>
		/// Mapping files used in the TestCase
		/// </summary>
		protected abstract IList Mappings { get; }

		/// <summary>
		/// Assembly to load mapping files from.
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "Nullables.Tests"; }
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			try
			{
				ExportSchema();
			}
			catch (Exception e)
			{
				log.Error("Error while setting up the database schema, ignoring the fixture", e);
				Assert.Ignore("Error while setting up the database schema: " + e.Message);
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
			bool fail = !wasClosed || !wasCleaned;

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
			int objectCount;
			using (ISession s = sessions.OpenSession())
			{
				objectCount = s.CreateQuery("from System.Object o").List().Count;
			}

			if (objectCount > 0)
			{
				log.Error("Test case didn't clean up the database after itself, re-creating the schema");
				DropSchema();
				ExportSchema();
				return false;
			}

			return true;
		}

		private void ExportSchema()
		{
			ExportSchema(Mappings, MappingsAssembly);
		}

		private void ExportSchema(IList files, string assemblyName)
		{
			cfg = new Configuration();

			for (int i = 0; i < files.Count; i++)
			{
				cfg.AddResource(assemblyName + "." + files[i].ToString(), Assembly.Load(assemblyName));
			}

			new SchemaExport(cfg).Create(OUTPUT_DDL, true);

			sessions = cfg.BuildSessionFactory();
		}

		/// <summary>
		/// Drops the schema that was built with the TestCase's Configuration.
		/// </summary>
		public void DropSchema()
		{
			new SchemaExport(cfg).Drop(OUTPUT_DDL, true);
		}

		protected ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}
	}
}