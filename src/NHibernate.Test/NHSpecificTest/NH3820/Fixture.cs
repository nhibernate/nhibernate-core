namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;
    using System.Collections;
    using System.Data;
    using System.Linq;
    using Criterion;
    using Dialect;
    using NUnit.Framework;
    using SqlCommand;

    #endregion

    [TestFixture]
    public class Fixture : TestCase
    {
        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get { return new[] { "NHSpecificTest.NH3820.Mappings.hbm.xml" }; }
        }

        protected override bool AppliesTo(Dialect dialect)
        {
            return dialect is MsSql2000Dialect;
        }

        protected override void OnSetUp()
        {
            using (var session = OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var product = new Product("T001", "T-Shirt");
                var product2 = new Product("U001", "Underwear");
                var variant = new ProductVariant("RED", "R001", product);
                var variant2 = new ProductVariant("YELLOW", "Y001", product2);

                product.AddVariant(variant);
                product2.AddVariant(variant2);
                session.Save(product);
                session.Save(product2);

                var user = new MembershipUser(25, "test@test.com", "OguzhanSoykan");
                var emailHistory = new MembershipUserEmailHistory(DateTime.Now, "test_old@test.com", user);
                var address = new MembershipUserAddress("221b baker street", user);
                var phone = new MembershipUserPhone("55544433322", user);
                var segment = new MembershipUserSegment("Rich User", DateTime.Now, user);
                user.AddAddress(address).AddEmailHistory(emailHistory).AddPhone(phone).AddSegment(segment);
                session.Save(user);

                var basket = new MembershipUserBasket(Guid.NewGuid(), DateTime.Now, user);
                basket.AddProduct(new MembershipUserBasketProduct(basket, DateTime.Now, product));
                basket.AddProduct(new MembershipUserBasketProduct(basket, DateTime.Now, product2));

                var orderline = new MembershipOrderLine(product, 8);
                var orderline2 = new MembershipOrderLine(product2, 2);
                var order = new MembershipOrder(user, basket).AddOrderLine(orderline).AddOrderLine(orderline2);

                session.Save(order);

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
        public void JoinWithHints_OnCriteria_MixedJoinTypesAndLockModes_IsolationLevelReadCommitted()
        {
            using (var session = OpenSession())
            using (session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var peopleCount =
                    session.CreateCriteria<MembershipUser>()
                           .CreateAlias("Phones", "ph", JoinType.InnerLoopJoin)
                           .CreateAlias("Segments", "se", JoinType.LeftOuterMergeJoin)
                           .CreateAlias("Addresses", "ad", JoinType.RightOuterHashJoin)
                           .CreateAlias("EmailHistories", "eh", JoinType.FullLoopJoin)
                           .CreateAlias("Orders", "o", JoinType.LeftOuterLoopJoin)
                           .CreateAlias("o.OrderLines", "ol", JoinType.FullHashJoin)
                           .CreateAlias("o.Basket", "ba", JoinType.LeftOuterJoin)
                           .SetLockMode(LockMode.UpgradeNoWait)
                           .SetLockMode("o", LockMode.Force)
                           .SetLockMode("ph", LockMode.Nolock)
                           .SetLockMode("se", LockMode.Upgrade)
                           .SetLockMode("ad", LockMode.Write)
                           .SetLockMode("eh", LockMode.Force)
                           .SetLockMode("ol", LockMode.Nolock)
                           .SetLockMode("ol", LockMode.Nolock)
                           .SetLockMode("ba", LockMode.Nolock)
                           .AddOrder(Order.Asc("o.Id"))
                           .List();

                Assert.That(peopleCount.Count, Is.EqualTo(2));
                Assert.That(peopleCount, Is.Ordered.By("Id"));
            }
        }

        [Test]
        public void JoinWithHints_OnCriteria_MixedJoinTypesAndLockModes_UnderWhereCondition()
        {
            using (var session = OpenSession())
            using (session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var peopleCount =
                    session.CreateCriteria<MembershipUser>()
                           .CreateAlias("Phones", "ph", JoinType.InnerLoopJoin)
                           .CreateAlias("Segments", "se", JoinType.LeftOuterMergeJoin)
                           .CreateAlias("Addresses", "ad", JoinType.RightOuterHashJoin)
                           .CreateAlias("EmailHistories", "eh", JoinType.FullLoopJoin)
                           .CreateAlias("Orders", "o", JoinType.LeftOuterLoopJoin)
                           .CreateAlias("o.OrderLines", "ol", JoinType.FullHashJoin)
                           .CreateAlias("o.Basket", "ba", JoinType.LeftOuterJoin).Add(Restrictions.Eq("ph.PhoneNumber", "55544433322"))
                           .SetLockMode(LockMode.UpgradeNoWait)
                           .SetLockMode("o", LockMode.Force)
                           .SetLockMode("ph", LockMode.Nolock)
                           .SetLockMode("se", LockMode.Upgrade)
                           .SetLockMode("ad", LockMode.Write)
                           .SetLockMode("eh", LockMode.Force)
                           .SetLockMode("ol", LockMode.Nolock)
                           .SetLockMode("ol", LockMode.Nolock)
                           .SetLockMode("ba", LockMode.Nolock)
                           .AddOrder(Order.Asc("o.Id"))
                           .List();

                Assert.That(peopleCount.Count, Is.EqualTo(2));
                Assert.That(peopleCount, Is.Ordered.By("Id"));
            }
        }

        [Test]
        public void JoinWithHints_OnQueryOver_MixedJoinTypesAndLockModes_ForgottenRootLock()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;
            MembershipUserBasket basketAlias = null;
            MembershipUserAddress addressAlias = null;
            MembershipUserPhone phoneAlias = null;
            MembershipUserSegment segmentAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;
            MembershipUserBasketProduct basketProductAlias = null;
            MembershipUserEmailHistory emailHistoryAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction(IsolationLevel.Serializable))
            {
                var people =
                    session.QueryOver<MembershipUser>()
                           .JoinAlias(x => x.Addresses, () => addressAlias, JoinType.RightOuterLoopJoin)
                           .JoinAlias(x => x.EmailHistories, () => emailHistoryAlias, JoinType.InnerMergeJoin)
                           .JoinAlias(x => x.Phones, () => phoneAlias, JoinType.FullHashJoin)
                           .JoinAlias(x => x.Segments, () => segmentAlias, JoinType.FullLoopJoin)
                           .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                           .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                           .JoinAlias(() => orderAlias.Basket, () => basketAlias, JoinType.InnerLoopJoin)
                           .Left.JoinAlias(() => basketAlias.BasketProducts, () => basketProductAlias)
                           .JoinAlias(() => basketProductAlias.Product, () => productAlias, JoinType.InnerHashJoin)
                           .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.RightOuterHashJoin)
                           .Lock().Nolock
                           .Lock().UpgradeNoWait
                           .Lock(() => orderAlias).Nolock
                           .Lock(() => orderLineAlias).UpgradeNoWait
                           .Lock(() => addressAlias).Nolock
                           .Lock(() => emailHistoryAlias).Nolock
                           .Lock(() => phoneAlias).Read
                           .Lock(() => segmentAlias).Force
                           .Lock(() => orderLineAlias).Upgrade
                           .Lock(() => basketAlias).Write
                           .Where(x => x.Age == 25).And(x => x.Email == "test@test.com")
                           .OrderBy(p => p.Age)
                           .Desc
                           .List();

                Assert.That(people.Count, Is.EqualTo(4));
                Assert.That(people, Is.Ordered.By("Age").Descending);
            }
        }

        [Test]
        public void JoinWithHints_OnQueryOver_MixedJoinTypesAndLockModes_RowCount()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;
            MembershipUserBasket basketAlias = null;
            MembershipUserAddress addressAlias = null;
            MembershipUserPhone phoneAlias = null;
            MembershipUserSegment segmentAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;
            MembershipUserBasketProduct basketProductAlias = null;
            MembershipUserEmailHistory emailHistoryAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {
                var peopleCount =
                    session.QueryOver<MembershipUser>()
                           .JoinAlias(x => x.Addresses, () => addressAlias, JoinType.RightOuterLoopJoin)
                           .JoinAlias(x => x.EmailHistories, () => emailHistoryAlias, JoinType.InnerMergeJoin)
                           .JoinAlias(x => x.Phones, () => phoneAlias, JoinType.FullHashJoin)
                           .JoinAlias(x => x.Segments, () => segmentAlias, JoinType.FullLoopJoin)
                           .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                           .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                           .JoinAlias(() => orderAlias.Basket, () => basketAlias, JoinType.InnerLoopJoin)
                           .Left.JoinAlias(() => basketAlias.BasketProducts, () => basketProductAlias)
                           .InnerHash.JoinAlias(() => basketProductAlias.Product, () => productAlias)
                           .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.RightOuterHashJoin)
                           .Lock().Nolock
                           .Lock().UpgradeNoWait
                           .Lock(() => orderAlias).Nolock
                           .Lock(() => orderLineAlias).UpgradeNoWait
                           .Lock(() => addressAlias).Nolock
                           .Lock(() => emailHistoryAlias).Nolock
                           .Lock(() => phoneAlias).Read
                           .Lock(() => segmentAlias).Force
                           .Lock(() => orderLineAlias).Upgrade
                           .Lock(() => basketAlias).Write
                           .Where(x => x.Age == 25).And(x => x.Email == "test@test.com")
                           .OrderBy(p => p.Age).Desc
                           .RowCount();

                Assert.That(peopleCount, Is.EqualTo(4));
            }
        }

        /// <summary>
        ///     The query over with hints_ complexed joins.
        ///     12 join
        /// </summary>
        [Test]
        public void JoinWithHints_OnQueryOver_MixedJoinTypesAndLockModes()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;
            MembershipUserBasket basketAlias = null;
            MembershipUserAddress addressAlias = null;
            MembershipUserPhone phoneAlias = null;
            MembershipUserSegment segmentAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;
            MembershipUserBasketProduct basketProductAlias = null;
            MembershipUserEmailHistory emailHistoryAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {
                var people =
                    session.QueryOver<MembershipUser>()
                           .JoinAlias(x => x.Addresses, () => addressAlias, JoinType.RightOuterLoopJoin)
                           .JoinAlias(x => x.EmailHistories, () => emailHistoryAlias, JoinType.InnerMergeJoin)
                           .JoinAlias(x => x.Phones, () => phoneAlias, JoinType.FullHashJoin)
                           .JoinAlias(x => x.Segments, () => segmentAlias, JoinType.FullLoopJoin)
                           .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterMergeJoin)
                           .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullMergeJoin)
                           .JoinAlias(() => orderAlias.Basket, () => basketAlias, JoinType.InnerLoopJoin)
                           .Left.JoinAlias(() => basketAlias.BasketProducts, () => basketProductAlias)
                           .JoinAlias(() => basketProductAlias.Product, () => productAlias, JoinType.InnerHashJoin)
                           .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.RightOuterHashJoin)
                           .Lock().Nolock
                           .Lock(() => orderAlias).Nolock
                           .Lock(() => orderLineAlias).UpgradeNoWait
                           .Lock(() => addressAlias).Nolock
                           .Lock(() => emailHistoryAlias).Nolock
                           .Lock(() => phoneAlias).Read
                           .Lock(() => segmentAlias).Force
                           .Lock(() => orderLineAlias).Upgrade
                           .Lock(() => basketAlias).Write
                           .Where(x => x.Age == 25).And(x => x.Email == "test@test.com")
                           .OrderBy(p => p.Age)
                           .Desc
                           .List();

                Assert.That(people.Count, Is.EqualTo(4));
                Assert.That(people, Is.Ordered.By("Age").Descending);
            }
        }

        [Test]
        public void JoinWithHints_OnQueryOver_MixedJoinTypesAndLockModes_UnderWhereAndOperation()
        {
            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;
            MembershipUserBasket basketAlias = null;
            MembershipUserAddress addressAlias = null;
            MembershipUserPhone phoneAlias = null;
            MembershipUserSegment segmentAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;
            MembershipUserBasketProduct basketProductAlias = null;
            MembershipUserEmailHistory emailHistoryAlias = null;

            using (var session = OpenSession())
            using (session.BeginTransaction())
            {
                var people =
                    session.QueryOver<MembershipUser>()
                           .RightLoop.JoinAlias(x => x.Addresses, () => addressAlias)
                           .InnerMerge.JoinAlias(x => x.EmailHistories, () => emailHistoryAlias)
                           .FullHash.JoinAlias(x => x.Phones, () => phoneAlias)
                           .FullLoop.JoinAlias(x => x.Segments, () => segmentAlias)
                           .LeftMerge.JoinAlias(x => x.Orders, () => orderAlias)
                           .FullMerge.JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias)
                           .InnerLoop.JoinAlias(() => orderAlias.Basket, () => basketAlias)
                           .Left.JoinAlias(() => basketAlias.BasketProducts, () => basketProductAlias)
                           .InnerHash.JoinAlias(() => basketProductAlias.Product, () => productAlias)
                           .RightHash.JoinAlias(() => productAlias.Variants, () => productVariantAlias)
                           .Lock().Nolock
                           .Lock(() => orderAlias).Nolock
                           .Lock(() => orderLineAlias).UpgradeNoWait
                           .Lock(() => addressAlias).Nolock
                           .Lock(() => emailHistoryAlias).Nolock
                           .Lock(() => phoneAlias).Read
                           .Lock(() => segmentAlias).Force
                           .Lock(() => orderLineAlias).Upgrade
                           .Lock(() => basketAlias).Write
                           .Where(x => x.Age == 25).And(x => x.Email == "test@test.com")
                           .OrderBy(p => p.Age)
                           .Desc
                           .List();

                Assert.That(people.Count, Is.EqualTo(4));
                Assert.That(people, Is.Ordered.By("Age").Descending);
            }
        }

        [Test]
        public void JoinWithHints_OnCriteria_WithMixedLockModes_IsolationLevelReadCommitted()
        {
            using (var session = OpenSession())
            using (session.BeginTransaction(IsolationLevel.ReadCommitted))
            {

                var items = session.CreateCriteria(typeof(MembershipUser))
                    .CreateAlias("Addresses", "addressAlias", JoinType.RightOuterLoopJoin)
                    .CreateAlias("EmailHistories", "emailHistoryAlias", JoinType.FullLoopJoin)
                    .CreateAlias("Phones", "phoneAlias", JoinType.InnerLoopJoin)
                    .CreateAlias("Segments", "segmentAlias", JoinType.LeftOuterMergeJoin)
                    .CreateAlias("Orders", "orderAlias", JoinType.LeftOuterLoopJoin)
                    .CreateAlias("orderAlias.OrderLines", "orderLineAlias", JoinType.FullHashJoin)
                    .CreateAlias("orderAlias.Basket", "basketAlias", JoinType.LeftOuterJoin)
                    .CreateAlias("basketAlias.BasketProducts", "basketProductAlias", JoinType.LeftOuterJoin)
                    .CreateAlias("basketProductAlias.Product", "productAlias", JoinType.LeftOuterJoin)
                    .CreateAlias("productAlias.Variants", "productVariantAlias", JoinType.LeftOuterJoin)
                    .SetLockMode(LockMode.Nolock)
                    .SetLockMode("orderAlias", LockMode.Force)
                    .SetLockMode("phoneAlias", LockMode.Nolock)
                    .SetLockMode("segmentAlias", LockMode.Upgrade)
                    .SetLockMode("addressAlias", LockMode.Write)
                    .SetLockMode("emailHistoryAlias", LockMode.Force)
                    .SetLockMode("orderLineAlias", LockMode.Nolock)
                    .SetLockMode("basketAlias", LockMode.Nolock)
                    .List();

                Assert.That(items.Count, Is.EqualTo(4));
            }

        }


        [Test]
        public void JoinWithHints_OnQueryOver_WithMixedLockModes_IsolationLevelReadCommitted()
        {
            using (var session = OpenSession())
            using (session.BeginTransaction(IsolationLevel.ReadCommitted))
            {

                MembershipOrder orderAlias = null;
                MembershipOrderLine orderLineAlias = null;
                MembershipUserBasket basketAlias = null;
                MembershipUserAddress addressAlias = null;
                MembershipUserPhone phoneAlias = null;
                MembershipUserSegment segmentAlias = null;
                Product productAlias = null;
                ProductVariant productVariantAlias = null;
                MembershipUserBasketProduct basketProductAlias = null;
                MembershipUserEmailHistory emailHistoryAlias = null;

                var items = session.QueryOver<MembershipUser>()
                        .JoinAlias(x => x.Addresses, () => addressAlias, JoinType.RightOuterLoopJoin)
                        .JoinAlias(x => x.EmailHistories, () => emailHistoryAlias, JoinType.FullLoopJoin)
                        .JoinAlias(x => x.Phones, () => phoneAlias, JoinType.InnerLoopJoin)
                        .JoinAlias(x => x.Segments, () => segmentAlias, JoinType.LeftOuterMergeJoin)
                        .JoinAlias(x => x.Orders, () => orderAlias, JoinType.LeftOuterLoopJoin)
                        .JoinAlias(() => orderAlias.OrderLines, () => orderLineAlias, JoinType.FullHashJoin)
                        .JoinAlias(() => orderAlias.Basket, () => basketAlias, JoinType.LeftOuterJoin)
                        .JoinAlias(() => basketAlias.BasketProducts, () => basketProductAlias, JoinType.LeftOuterJoin)
                        .JoinAlias(() => basketProductAlias.Product, () => productAlias, JoinType.LeftOuterJoin)
                        .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.LeftOuterJoin)
                        .Lock().Nolock
                        .Lock(() => orderAlias).Force
                        .Lock(() => phoneAlias).Read
                        .Lock(() => phoneAlias).Nolock
                        .Lock(() => orderLineAlias).UpgradeNoWait
                        .Lock(() => addressAlias).Write
                        .Lock(() => emailHistoryAlias).Nolock
                        .Lock(() => segmentAlias).Upgrade
                        .Lock(() => orderLineAlias).Nolock
                        .Lock(() => emailHistoryAlias).Force
                        .Lock(() => basketAlias).Nolock
                        .List();

                Assert.That(items.Count(), Is.EqualTo(4));
            }

        }
    }
}
