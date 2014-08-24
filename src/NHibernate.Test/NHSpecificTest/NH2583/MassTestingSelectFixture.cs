using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public class MassTestingSelectFixture : AbstractMassTestingFixture
    {
        protected override int TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
        {
            IQueryable<int?> result = session.Query<MyBO>().Where(condition).Select(bo => (int?) bo.BO1.Id);
            
            var forceDBRun = result.ToList();
            
            IEnumerable<int> resultNullTo0 = forceDBRun.Select(i => i ?? 0);

            var expectedBO1Ids = session.Query<MyBO>().Where(bo => expectedIds.Contains(bo.Id)).Select(bo => bo.BO1 == null ? 0 : bo.BO1.Id).ToList();
            AreEqual(expectedBO1Ids, resultNullTo0.ToArray());
            
            // Unused result.
            return -1;
        }

        // Condition pattern: (A && B) && (C || D) SELECT E

        [Test]
        public void Test_xyP_in_E____xy_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                // The last setter forces MyBOs to have a pointer to a BO1 so that we can check
                // that the right BO1.Ids are returned by the Select(bo => (int?) bo.BO1.Id)!
                    Setters<TK, TK, TK, TBO1_I>(MyBO.SetK1, MyBO.SetK2, MyBO.SetK3, MyBO.SetBO1_I1));
        }

        [Test]
        public void Test_xy_in_A__xyP_in_E____xy_OJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                // Here, SetBO1_I1 is already called to populate the value for the expression -
                // therefore, an additional call to force BO1 objects is not necessary.
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A__xyP_in_E____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.K2 == 1 || x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A_C_E____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_A_C_D_E____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.BO1.Id > 0 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.BO1.Id > 0 && x.K3 == 1),
                Setters<TBO1_I, TK, TK, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_C_D_E____xy_IJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.BO1.Id > 0 && x.K3 == 1),
                Setters<TK, TK, TBO1_I, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_C_E____xy_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K1 == 1) && (x.BO1.Id > 0 && x.K2 == 1 || x.K3 == 1),
                Setters<TK, TK, TBO1_I, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetK3));
        }
    }
}
