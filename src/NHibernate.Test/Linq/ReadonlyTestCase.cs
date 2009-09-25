using System;
using System.Collections;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Classic;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    public abstract class ReadonlyTestCase
    {
        private const bool OutputDdl = false;
        protected Configuration _cfg;
        protected ISessionFactoryImplementor _sessions;

        private static readonly ILog log = LogManager.GetLogger(typeof(TestCase));

        protected Dialect.Dialect Dialect
        {
            get { return NHibernate.Dialect.Dialect.GetDialect(_cfg.Properties); }
        }

        /// <summary>
        /// To use in in-line test
        /// </summary>
        protected bool IsClassicParser
        {
            get
            {
                return _sessions.Settings.QueryTranslatorFactory is ClassicQueryTranslatorFactory;
            }
        }

        /// <summary>
        /// To use in in-line test
        /// </summary>
        protected bool IsAntlrParser
        {
            get
            {
                return _sessions.Settings.QueryTranslatorFactory is ASTQueryTranslatorFactory;
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

        static ReadonlyTestCase()
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

                BuildSessionFactory();
                //CreateSchema();
                if (!AppliesTo(_sessions))
                {
                    DropSchema();
                    Cleanup();
                    Assert.Ignore(GetType() + " does not apply with the current session-factory configuration");
                }

                //OnFixtureSetup();
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
            OnFixtureTeardown();
 
			//DropSchema();
            Cleanup();
        }

        protected virtual void OnFixtureSetup()
        {
        }

        protected virtual void OnFixtureTeardown()
        {
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
            bool wereConnectionsClosed = CheckConnectionsWereClosed();
            bool fail = !wasClosed || !wereConnectionsClosed;

            if (fail)
            {
                Assert.Fail("Test didn't clean up after itself. session closed: " + wasClosed + " connection closed: " + wereConnectionsClosed);
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
            _cfg = new Configuration();
            if (TestConfigurationHelper.hibernateConfigFile != null)
                _cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

            Assembly assembly = Assembly.Load(MappingsAssembly);

            foreach (string file in Mappings)
            {
                _cfg.AddResource(MappingsAssembly + "." + file, assembly);
            }

            Configure(_cfg);
        }

        protected virtual void CreateSchema()
        {
            new SchemaExport(_cfg).Create(OutputDdl, true);
        }

        private void DropSchema()
        {
            new SchemaExport(_cfg).Drop(OutputDdl, true);
        }

        protected virtual void BuildSessionFactory()
        {
            _sessions = (ISessionFactoryImplementor)_cfg.BuildSessionFactory();
            connectionProvider = _sessions.ConnectionProvider as DebugConnectionProvider;
        }

        private void Cleanup()
        {
            if (_sessions != null)
            {
                _sessions.Close();
            }
            _sessions = null;
            connectionProvider = null;
            lastOpenedSession = null;
            _cfg = null;
        }

        protected ISessionFactoryImplementor Sfi
        {
            get { return _sessions; }
        }

        protected virtual ISession OpenSession()
        {
            lastOpenedSession = _sessions.OpenSession();
            return lastOpenedSession;
        }

        protected virtual ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            lastOpenedSession = _sessions.OpenSession(sessionLocalInterceptor);
            return lastOpenedSession;
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