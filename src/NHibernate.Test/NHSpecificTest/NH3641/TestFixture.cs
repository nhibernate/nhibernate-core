using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3641
{
    public class TestFixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            using (var session = OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var child = new Entity { Id = 1, Flag = false };
                var parent = new Entity { Id = 2, ChildInterface = child, ChildConcrete = child };

                session.Save(child);
                session.Save(parent);

                tx.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (var session = OpenSession())
            using (var tx = session.BeginTransaction())
            {
                DeleteAll<Entity>(session);
                tx.Commit();
            }
        }

        private static void DeleteAll<T>(ISession session)
        {
            session.CreateQuery("delete from " + typeof(T).Name).ExecuteUpdate();
        }

        [Test]
        public void TrueOrChildPropertyConcrete()
        {
            using (var session = OpenSession())
            {
                var result = session.Query<IEntity>().Where(x => true || x.ChildConcrete.Flag).ToList();
                Assert.That(result, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void TrueOrChildPropertyInterface()
        {
            using (var session = OpenSession())
            {
                var result = session.Query<IEntity>().Where(x => true || ((Entity)x.ChildInterface).Flag).ToList();
                Assert.That(result, Has.Count.EqualTo(2));
            }
        }
    }
}
