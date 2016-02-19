namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;
    using System.Collections;

    using NHibernate.Criterion;
    using NHibernate.Dialect;
    using NHibernate.SqlCommand;

    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class Fixture : TestCase
    {
        protected override string MappingsAssembly
        {
            get
            {
                return "NHibernate.Test";
            }
        }

        protected override IList Mappings
        {
            get
            {
                return new[] { "NHSpecificTest.NH3820.Mappings.hbm.xml" };
            }
        }

        protected override bool AppliesTo(Dialect dialect)
        {
            return dialect is MsSql2000Dialect;
        }

        protected override void OnSetUp()
        {
            using (var session = this.OpenSession())
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
            using (var session = this.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete("from System.Object");
                session.Flush();
                transaction.Commit();
            }
        }

        /// <summary>
        /// The query over with hints_ complexed joins.
        /// 12 join
        /// </summary>
        [Test]
        public void QueryOverWithHintsComplexedJoins()
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

            using (var session = this.OpenSession())
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
        public void QueryOverWithHints_Criteria()
        {
            using (var session = this.OpenSession())
            using (session.BeginTransaction())
            {
                var peopleCount =
                    session.CreateCriteria<MembershipOrder>()
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

        [Test]
        public void QueryOverWithHints_ForgottenLock()
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

            using (var session = this.OpenSession())
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
        public void QueryOverWithHints_RowCount()
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

            using (var session = this.OpenSession())
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
                           .JoinAlias(() => basketProductAlias.Product, () => productAlias, JoinType.InnerHashJoin)
                           .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.RightOuterHashJoin)
                           .Lock()
                           .Nolock.Lock()
                           .UpgradeNoWait.Lock(() => orderAlias)
                           .Nolock.Lock(() => orderLineAlias)
                           .UpgradeNoWait.Lock(() => addressAlias)
                           .Nolock.Lock(() => emailHistoryAlias)
                           .Nolock.Lock(() => phoneAlias)
                           .Read.Lock(() => segmentAlias)
                           .Force.Lock(() => orderLineAlias)
                           .Upgrade.Lock(() => basketAlias)
                           .Write.Where(x => x.Age == 25)
                           .And(x => x.Email == "test@test.com")
                           .OrderBy(p => p.Age)
                           .Desc.RowCount();

                Assert.That(peopleCount, Is.EqualTo(4));
            }
        }
    }
}
