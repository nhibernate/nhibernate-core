using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH2903
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var e1 = new Entity2903();
                    e1.Name = "Bob";
                    e1.Children.Add(new Child { Code = "c1" });
                    e1.Children.Add(new Child { Code = "c2" });
                    e1.Children.Add(new Child { Code = "c3" });
                    session.Save(e1);

                    

                    session.Flush();
                    transaction.Commit();
                }
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete("from System.Object");

                    session.Flush();
                    transaction.Commit();
                }
            }
        }

        [Test]
        public void CreateFilterSelectCountShouldReturnCount()
        {
            using (ISession session = OpenSession())
            {
                using (session.BeginTransaction())
                {
                    
                    var result = session.Query<Entity2903>()
                                 .FirstOrDefault();
                    Assert.IsNotNull(result);
                    int count = session.CreateFilter(result.Children, "select count(*)").List<int>()[0];
                    Assert.AreEqual(3,count);
                }
            }
        }
        [Test]
        public void CreateFilterShouldAllowCollectionPaging()
        {
            using (ISession session = OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var result = session.Query<Entity2903>()
                                 .FirstOrDefault();
                    Assert.IsNotNull(result);
                    var children = session.CreateFilter(result.Children,string.Empty)
                        .SetFirstResult(1)
                        .SetMaxResults(2)
                        .List();
                    Assert.AreEqual(2, children.Count);
                }
            }
        }
    }
}
