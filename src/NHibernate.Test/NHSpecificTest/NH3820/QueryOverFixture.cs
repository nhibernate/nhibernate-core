namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using Criteria.Lambda;
    using Criterion;
    using NUnit.Framework;
    using SqlCommand;

    #endregion

    [TestFixture]
    public class QueryOverFixture : LambdaFixtureBase
    {
        [Test]
        public void SubCriteria_NoAlias_WithRootNolock()
        {
            ICriteria expected =
                CreateTestCriteria(typeof (MembershipUser)).SetLockMode(LockMode.Nolock)
                                                           .Add(Restrictions.Eq("Name", "test name"))
                                                           .Add(Restrictions.Not(Restrictions.Eq("Name", "not test name")))
                                                           .Add(Restrictions.Gt("Age", 10))
                                                           .Add(Restrictions.Ge("Age", 11))
                                                           .Add(Restrictions.Lt("Age", 50))
                                                           .Add(Restrictions.Le("Age", 49))
                                                           .Add(Restrictions.Eq("class", typeof (MembershipUser)))
                                                           .Add(Restrictions.Eq("class", typeof (MembershipUser).FullName));

            IQueryOver<MembershipUser> actual =
                CreateTestQueryOver<MembershipUser>().Lock().Nolock
                                                     .And(p => p.Name == "test name")
                                                     .And(p => p.Name != "not test name")
                                                     .And(p => p.Age > 10)
                                                     .And(p => p.Age >= 11)
                                                     .And(p => p.Age < 50)
                                                     .And(p => p.Age <= 49)
                                                     .And(p => p.GetType() == typeof (MembershipUser))
                                                     .And(p => p is MembershipUser);

            AssertCriteriaAreEqual(expected, actual);
        }

        [Test]
        public void SubCriteria_JoinQueryOverCombinations_WithMixedLockModes()
        {
            ICriteria expected =
                CreateTestCriteria(typeof (MembershipUser))
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
                    .SetLockMode("basketAlias", LockMode.Nolock);

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

            IQueryOver<MembershipUser> actual =
                CreateTestQueryOver<MembershipUser>()
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
                    .Lock(() => basketAlias).Nolock;

            AssertCriteriaAreEqual(expected, actual);
        }

        [Test]
        public void SubCriteria_JoinQueryOverCombinations_WithMixedLockModes_OnClause()
        {
            ICriteria expected =
                CreateTestCriteria(typeof (MembershipUser))
                    .CreateCriteria("Orders", "orderAlias", JoinType.LeftOuterLoopJoin, Restrictions.Gt("Id", 0))
                    .CreateCriteria("OrderLines", "orderLineAlias", JoinType.FullHashJoin, Restrictions.Gt("Id", 0))
                    .CreateCriteria("Product", "productAlias", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "TestName"))
                    .CreateCriteria("Variants", "productVariantAlias", JoinType.LeftOuterJoin, Restrictions.Eq("Code", "testVariant"))
                    .SetLockMode(LockMode.Nolock)
                    .SetLockMode("orderAlias", LockMode.Force)
                    .SetLockMode("productAlias", LockMode.Nolock)
                    .SetLockMode("productVariantAlias", LockMode.Force)
                    .SetLockMode("orderLineAlias", LockMode.UpgradeNoWait);

            MembershipOrder orderAlias = null;
            MembershipOrderLine orderLineAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;

            IQueryOver<MembershipUser> actual =
                CreateTestQueryOver<MembershipUser>()
                    .LeftLoop.JoinQueryOver(x => x.Orders, () => orderAlias, x => x.Id > 0)
                    .FullHash.JoinQueryOver(x => x.OrderLines, () => orderLineAlias, x => x.Id > 0)
                    .Left.JoinQueryOver(x => x.Product, () => productAlias, x => x.Name == "TestName")
                    .Left.JoinQueryOver(x => x.Variants, () => productVariantAlias, x => x.Code == "testVariant")
                    .Lock().Nolock
                    .Lock(() => orderAlias).Force
                    .Lock(() => productAlias).Nolock
                    .Lock(() => productVariantAlias).Force
                    .Lock(() => orderLineAlias).UpgradeNoWait;

            AssertCriteriaAreEqual(expected, actual);
        }
    }
}
