using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NHibernate.Test.NHSpecificTest.NH1584;
using NUnit.Framework;

namespace NHibernate.Test.Events
{
    [TestFixture]
    public class EntityListenerIntegrationTest : TestCase
    {
        private ISession _session;
        private ITransaction _transaction;

        [SetUp]
        public void StartSessionAndTx()
        {
            _session = OpenSession();
            _transaction = _session.BeginTransaction();            
        }

        [TearDown]
        public void CleanupSessionAndTx()
        {
            var tx = _session.BeginTransaction();
            _session.Delete("from MyEntityForTesting");
            tx.Commit();
            _session.Close();
        }

        [Test]
        public void PreUpdate_ChangesProperty()
        {
            _session.Persist(new MyEntityForTesting());
            _transaction.Commit();

            var refreshedEntity = _session.Query<MyEntityForTesting>().Single();

            _transaction = _session.BeginTransaction();
            refreshedEntity.SomeProperty = "updateme";
            _session.Update(refreshedEntity);
            _transaction.Commit();

            refreshedEntity = _session.Query<MyEntityForTesting>().Single();
            Assert.AreEqual("preupdate", refreshedEntity.SomeProperty);
        }

        [Test]
        public void PreInsert_ChangesProperty()
        {
            _session.Persist(new MyEntityForTesting());
            _transaction.Commit();

            var refreshedEntity = _session.Query<MyEntityForTesting>().Single();
            Assert.AreEqual("preinsert", refreshedEntity.SomeProperty);
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get { return new string[] { "Events.MyEntityForTesting.hbm.xml" }; }
        }
    }
}
