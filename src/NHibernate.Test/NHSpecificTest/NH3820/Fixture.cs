#region using

using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NUnit.Framework;

#endregion

namespace NHibernate.Test.NHSpecificTest.NH3820
{
    [TestFixture]
    public class Fixture : TestCase
    {
        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get { return new[] { "NHSpecificTest.NH3820.MembershipUser.hbm.xml" }; }
        }

        protected override void OnSetUp()
        {
            using (var session = OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var u1 = new MembershipUser(25, "test@test.com", "OguzhanSoykan");
                var order = new MembershipOrder(u1);
                var orderline = new MembershipOrderLine(order, "Kazak1");
                order.AddOrderLine(orderline);
                u1.AddOrder(order);

                var u2 = new MembershipUser(25, "test2@test.com", "OguzhanSoykan_2");
                var order2 = new MembershipOrder(u2);
                var orderline2 = new MembershipOrderLine(order2, "Gomlek");
                order2.AddOrderLine(orderline2);
                u2.AddOrder(order2);

                session.Save(u1);
                session.Save(u2);
                session.Flush();
                transaction.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (var session = OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete("from System.Object");
                session.Flush();
                transaction.Commit();
            }
        }

        [Test]
        public void QueryOverWithHints()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {

                var people = session.QueryOver<MembershipUser>()
                                    .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                                    .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                                    .Lock(() => orderAlias).Nolock
                                    .Lock().Nolock
                                    .Lock(() => orderLineAlias).UpgradeNoWait
                                    .Where(x => x.Age == 25)
                                    .OrderBy(p => p.Age)
                                    .Desc
                                    .List();

                Assert.That(people.Count, Is.EqualTo(2));
                Assert.That(people, Is.Ordered.By("Age").Descending);
            }
        }

        [Test]
        public void QueryOverWithHints_ForgottenLock()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {

                var people = session.QueryOver<MembershipUser>()
                                    .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                                    .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                                    .Lock(() => orderAlias).Nolock
                                    .Lock().Nolock
                                    .Lock().Upgrade
                                    .Lock(() => orderLineAlias).UpgradeNoWait
                                    .OrderBy(p => p.Age)
                                    .Desc
                                    .List();

                Assert.That(people.Count, Is.EqualTo(2));
                Assert.That(people, Is.Ordered.By("Age").Descending);
            }
        }

        [Test]
        public void QueryOverWithHints_RowCount()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {

                var peopleCount = session.QueryOver<MembershipUser>()
                                    .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                                    .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                                    .Where(x => x.Age == 25)
                                    .RowCount();

                Assert.That(peopleCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void QueryOverWithHints_Criteria()
        {
            using (var session = OpenSession())
            using (session.BeginTransaction())
            {
                IList peopleCount = session.CreateCriteria<MembershipOrder>()
                                              .CreateAlias("User", "u", JoinType.LeftOuterHashJoin)
                                              .CreateAlias("OrderLines", "ol", JoinType.LeftOuterMergeJoin)
                                              .SetLockMode("ol", LockMode.Nolock)
                                              .SetLockMode(LockMode.UpgradeNoWait)
                                              .SetLockMode("u", LockMode.Force)
                                              .AddOrder(Order.Asc("u.Id"))
                                              .List();

                Assert.That(peopleCount.Count, Is.EqualTo(2));
                Assert.That(peopleCount, Is.Ordered.By("Id"));
            }
        }

    }
}
