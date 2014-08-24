using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public class MassTestingOneOrTreeFixture : AbstractMassTestingFixture
    {
        protected override int TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
        {
            var result = session.Query<MyBO>().Where(condition);
            AreEqual(expectedIds, result.Select(bo => bo.Id).ToArray());
            return expectedIds.Count();
        }

        // Condition pattern: (A && B) && (C || D)

        #region 1a. One path

        [Test]
        public void Test_xyP_in_A_C____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && (x.BO1.I2 == 1 || x.K2 == 1),
                    Setters<TBO1_I, TK, TBO1_I, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetBO1_I2, MyBO.SetK2));
        }

        [Test]
        public void Test_xyP_in_A_C_D____xy_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && (x.BO1.I2 == 1 || x.BO1.I3 == 1),
                    Setters<TBO1_I, TK, TBO1_I, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetBO1_I2, MyBO.SetBO1_I3));
        }

        [Test]
        public void Test_xyP_in_C_D____xy_IJ()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && (x.BO1.I1 == 1 || x.BO1.I2 == 1),
                    Setters<TK, TK, TBO1_I, TBO1_I>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetBO1_I2));
        }

        [Test]
        public void Test_xyP_in_C____xy_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && (x.BO1.I1 == 1 || x.K3 == 1),
                    Setters<TK, TK, TBO1_I, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetK3));
        }

        #endregion 1a. One path

        #region 1b. Two paths
        [Test]
        public void Test_xyP_in_A_C__rsQ_in_C____xy_IJ_rs_OJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && ((x.BO1.I2 == 1 && x.BO2.J1 == 1) || x.K2 == 1),
                    Setters<TBO1_I, TK, TBO1_I, TBO2_J, TK>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetBO1_I2, MyBO.SetBO2_J1, MyBO.SetK2));
        }

        [Test]
        public void Test_xyP_in_A_C__rsQ_in_C_D____xy_IJ_rs_IJ()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && ((x.BO1.I2 == 1 && x.BO2.J1 == 1) || x.BO2.J2 == 1),
                    Setters<TBO1_I, TK, TBO1_I, TBO2_J, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetBO1_I2, MyBO.SetBO2_J1, MyBO.SetBO2_J2));
        }

        [Test]
        public void Test_xyP_in_C_D__rsQ_in_C_D____xy_IJ_rs_IJ()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && ((x.BO1.I1 == 1 && x.BO2.J1 == 1) || (x.BO1.I2 == 1 && x.BO2.J2 == 1)),
                    Setters<TK, TK, TBO1_I, TBO2_J, TBO1_I, TBO2_J>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetBO2_J1, MyBO.SetBO1_I2, MyBO.SetBO2_J2));
        }

        [Test]
        public void Test_xyP_in_C__rsQ_in_C____xy_OJ_rs_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && ((x.BO1.I1 == 1 && x.BO2.J1 == 1) || x.K3 == 1),
                    Setters<TK, TK, TBO1_I, TBO2_J, TK>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetBO2_J1, MyBO.SetK3));
        }

        [Test]
        public void Test_xyP_in_C__rsQ_in_D____xy_OJ_rs_OJ()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && (x.BO1.I1 == 1 || x.BO2.J1 == 1),
                    Setters<TK, TK, TBO1_I, TBO2_J>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetBO2_J1));
        }


        #endregion 1b. Two paths

        #region 1c. Path and subpath
        [Test]
        public void Test_wxyP_in_A_C____IJ_wx_IJ_wxy()
        {
            RunTest(x => (x.BO1.BO2.J1 == 1 && x.K1 == 1) && (x.BO1.BO2.J2 == 1 || x.K3 == 1),
                    Setters<TBO1_BO2_J, TK, TBO1_BO2_J, TK>(MyBO.SetBO1_BO2_J1, MyBO.SetK1, MyBO.Set_BO1_BO2_J2, MyBO.SetK3));
        }

        [Test]
        public void Test_wxQ_in_C__wxyP_in_D____IJ_wx_OJ_wxy()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && (x.BO1.I1 == 1 || x.BO1.BO2.J1 == 1),
                    Setters<TK, TK, TBO1_I, TBO1_BO2_J>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_I1, MyBO.SetBO1_BO2_J1));
        }

        [Test]
        public void Test_wxQ_in_A_wxyP_D____IJ_wx_OJ_wxy()
        {
            RunTest(x => (x.BO1.I1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.BO1.BO2.J1 == 1),
                    Setters<TBO1_I, TK, TK, TBO1_BO2_J>(MyBO.SetBO1_I1, MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_BO2_J1));
        }

        #endregion 1c. Path and subpath

        #region 1d. Partially overlapping paths

        [Test]
        public void Test_wxvP_in_C__wxyP_in_D____IJ_wx_OJ_wxv_OJ_wxy()
        {
            RunTest(x => (x.K1 == 1 && x.K2 == 1) && (x.BO1.BO2.J1 == 1 || x.BO1.BO3.L1 == 1),
                    Setters<TK, TK, TBO1_BO2_J, TBO1_BO3_L>(MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_BO2_J1, MyBO.SetBO1_BO3_L1));
        }

        [Test]
        public void Test_wxvP_in_A__wxyP_in_D____IJ_wx_IJ_wxv_OJ_wxy()
        {
            RunTest(x => (x.BO1.BO2.J1 == 1 && x.K1 == 1) && (x.K2 == 1 || x.BO1.BO3.L1 == 1),
                    Setters<TBO1_BO2_J, TK, TK, TBO1_BO3_L>(MyBO.SetBO1_BO2_J1, MyBO.SetK1, MyBO.SetK2, MyBO.SetBO1_BO3_L1));
        }

        #endregion 1d. Partially overlapping paths
    }
}
