using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class ProjectionsTests : LinqTestCase
    {
        [Test]
        public void ProjectAnonymousTypeWithWhere()
        {
            var query = (from user in db.Users
                         where user.Name == "ayende"
                         select user.Name)
                .First();
            Assert.AreEqual("ayende", query);
        }


        [Test]
        public void ProjectConditionals()
        {
            var query = (from user in db.Users
                         orderby user.Id
                         select new { user.Id, GreaterThan2 = user.Id > 2 ? "Yes" : "No" })
                .ToList();
            Assert.AreEqual("No", query[0].GreaterThan2);
            Assert.AreEqual("No", query[1].GreaterThan2);
            Assert.AreEqual("Yes", query[2].GreaterThan2);
        }

        [Test]
        public void ProjectAnonymousTypeWithMultiply()
        {
            var query = (from user in db.Users
                         select new { user.Name, user.Id, Id2 = user.Id * 2 })
                .ToList();
            Assert.AreEqual(3, query.Count);
            foreach (var user in query)
            {
                Assert.AreEqual(user.Id * 2, user.Id2);
            }
        }

        [Test]
        public void ProjectAnonymousTypeWithSubstraction()
        {
            var query = (from user in db.Users
                         select new { user.Name, user.Id, Id2 = user.Id - 2 })
                .ToList();
            Assert.AreEqual(3, query.Count);
            foreach (var user in query)
            {
                Assert.AreEqual(user.Id - 2, user.Id2);
            }
        }

        [Test]
        public void ProjectAnonymousTypeWithDivision()
        {
            var query = (from user in db.Users
                         select new { user.Name, user.Id, Id2 = (user.Id * 10) / 2 })
                .ToList();
            Assert.AreEqual(3, query.Count);
            foreach (var user in query)
            {
                Assert.AreEqual((user.Id * 10) / 2, user.Id2);
            }
        }

        [Test]
        public void ProjectAnonymousTypeWithAddition()
        {
            var query = (from user in db.Users
                         select new { user.Name, user.Id, Id2 = (user.Id + 101) })
                .ToList();
            Assert.AreEqual(3, query.Count);
            foreach (var user in query)
            {
                Assert.AreEqual((user.Id + 101), user.Id2);
            }
        }

        [Test]
        public void ProjectAnonymousTypeAndConcatenateFields()
        {
            var query = (from user in db.Users
                         orderby user.Name
                         select new { DoubleName = user.Name + " " + user.Name, user.RegisteredAt }

                        )
                .ToList();

            Assert.AreEqual("ayende ayende", query[0].DoubleName);
            Assert.AreEqual("nhibernate nhibernate", query[1].DoubleName);
            Assert.AreEqual("rahien rahien", query[2].DoubleName);


            Assert.AreEqual(DateTime.Today, query[0].RegisteredAt);
            Assert.AreEqual(new DateTime(2000, 1, 1), query[1].RegisteredAt);
            Assert.AreEqual(new DateTime(1998, 12, 31), query[2].RegisteredAt);
        }

        [Test]
        public void ProjectKnownType()
        {
            var query = (from user in db.Users
                         orderby user.Id
                         select new KeyValuePair<string, DateTime>(user.Name, user.RegisteredAt))
                .ToList();

            Assert.AreEqual("ayende", query[0].Key);
            Assert.AreEqual("rahien", query[1].Key);
            Assert.AreEqual("nhibernate", query[2].Key);


            Assert.AreEqual(DateTime.Today, query[0].Value);
            Assert.AreEqual(new DateTime(1998, 12, 31), query[1].Value);
            Assert.AreEqual(new DateTime(2000, 1, 1), query[2].Value);
        }

        [Test]
        public void ProjectAnonymousType()
        {
            var query = (from user in db.Users
                         orderby user.Id
                         select new { user.Name, user.RegisteredAt })
                .ToList();
            Assert.AreEqual("ayende", query[0].Name);
            Assert.AreEqual("rahien", query[1].Name);
            Assert.AreEqual("nhibernate", query[2].Name);


            Assert.AreEqual(DateTime.Today, query[0].RegisteredAt);
            Assert.AreEqual(new DateTime(1998, 12, 31), query[1].RegisteredAt);
            Assert.AreEqual(new DateTime(2000, 1, 1), query[2].RegisteredAt);
        }

        [Test]
        public void ProjectUserNames()
        {
            var query = (from user in db.Users
                         select user.Name).ToList();
            Assert.AreEqual(3, query.Count);
            Assert.AreEqual(3, query.Intersect(new[] { "ayende", "rahien", "nhibernate" })
                                   .ToList().Count);
        }
    }
}