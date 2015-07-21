using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public class MassTestingOrderByFixture : AbstractMassTestingFixture
    {
        protected override int TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
        {
            IQueryable<MyBO> result = session.Query<MyBO>().Where(condition).OrderByDescending(bo => bo.BO1.I1 ?? bo.BO1.Id);

            var forceDBRun = result.ToList();

            AreEqual(expectedIds, forceDBRun.Select(bo => bo.Id).ToArray());

            return expectedIds.Count();
        }

        // Condition pattern: (A && B) && (C || D) ORDER BY F

        [Test]
        public void Test_xyP_in_F____xy_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                // The last setter forces MyBOs to have a pointer to a BO1 so that we can check
                // that the right BO1.Ids are returned by the Select(bo => (int?) bo.BO1.Id)!
                    Setters<TK, TK, TK, TBO1_I>(MyBO.SetK1, MyBO.SetK2, MyBO.SetK3, MyBO.SetBO1_I1));
        }

        [Test]
        public void Test_xy_in_A__xyP_in_F____xy_OJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                // Here, SetBO1_I1 is already called to populate the value for the expression -
                // therefore, an additional call to force BO1 objects is not necessary.
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A__xyP_in_F____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A_C_F____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A_C_D_F____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.BO1.Id > 0 && x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_C_D_F____xy_IJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.BO1.Id > 0 && x.K3 == 1),
                Setters<TK, TK, TBO1_I, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_C_F____xy_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.K3 == 1),
                Setters<TK, TK, TBO1_I, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetK3));
        }
    }
}
