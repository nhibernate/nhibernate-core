using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
	[TestFixture]
	public class MassTestingMoreOperatorsFixture : AbstractMassTestingFixture
    {
        protected override int TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
        {
            IQueryable<int?> result = session.Query<MyBO>().Where(condition).Select(bo => (int?)bo.BO1.Id);

            var forceDBRun = result.ToList();

            IEnumerable<int> resultNullTo0 = forceDBRun.Select(i => i ?? 0);

            var expectedBO1Ids = session.Query<MyBO>().Where(bo => expectedIds.Contains(bo.Id)).Select(bo => bo.BO1 == null ? 0 : bo.BO1.Id).ToList();
            AreEqual(expectedBO1Ids, resultNullTo0.ToArray());

            // Unused result.
            return -1;
        }

        // Condition pattern: (A && B) && (C || D) SELECT E

        [Test]
        public void TestNestedPlus()
        {
            RunTest(x => (x.K1 + x.K2) + x.K2 == null || (x.K1 + x.K2) + x.K2 == null,
                    Setters<TK, TK>(MyBO.SetK1, MyBO.SetK2));
        }

        [Test]
        public void TestNestedPlusBehindNot()
        {
            RunTest(x => !((x.K1 + x.K2) + x.K2 != null),
                    Setters<TK, TK>(MyBO.SetK1, MyBO.SetK2));
        }

        [Test]
        public void TestNestedPlusBehindNotAnd()
        {
            RunTest(x => !((x.K1 + x.K2) + x.K2 != null && (x.K1 + x.K2) + x.K2 != null),
                    Setters<TK, TK>(MyBO.SetK1, MyBO.SetK2));
        }

        [Test]
        public void TestNestedPlusBehindNotOr()
        {
            RunTest(x => !((x.K1 + x.K2) + x.K2 != null || (x.K1 + x.K2) + x.K2 != null),
                    Setters<TK, TK>(MyBO.SetK1, MyBO.SetK2));
        }

        [Test]
        public void TestNestedPlusBehindOrNav()
        {
            RunTest(x => (x.BO1.I1 + x.BO1.I2) + x.BO1.I2 == null || (x.BO1.I1 + x.BO1.I2) + x.BO1.I2 == null,
                    Setters<TBO1_I, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO1_I2));
        }
        [Test]
        public void TestNestedPlusBehindNotNav()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO1.I2 != null),
                    Setters<TBO1_I, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO1_I2));
        }
        [Test]
        public void TestNestedPlusBehindNotAndNav()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO1.I2 != null && (x.BO1.I1 + x.BO1.I2) + x.BO1.I2 != null),
                    Setters<TBO1_I, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO1_I2));
        }
        [Test]
        public void TestNestedPlusBehindNotOrNav()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO1.I2 != null || (x.BO1.I1 + x.BO1.I2) + x.BO1.I2 != null),
                    Setters<TBO1_I, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO1_I2));
        }

        [Test]
        public void TestNestedPlusBehindOrNav2()
        {
            RunTest(x => (x.BO1.I1 + x.BO1.I2) + x.BO1.I2 == null || (x.BO2.J1 + x.BO2.J2) + x.BO2.J2 == null,
                    Setters<TBO1_I, TBO1_I, TBO2_J, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO1_I2, MyBO.SetBO2_J1, MyBO.SetBO2_J2));
        }
        [Test]
        public void TestNestedPlusBehindNotOrNav2()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO1.I2 == null || (x.BO2.J1 + x.BO2.J2) + x.BO2.J2 == null),
                    Setters<TBO1_I, TBO1_I, TBO2_J, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO1_I2, MyBO.SetBO2_J1, MyBO.SetBO2_J2));
        }
        [Test]
        public void TestNestedPlusBehindNotAndNav2()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO1.I2 == null && (x.BO2.J1 + x.BO2.J2) + x.BO2.J2 == null),
                    Setters<TBO1_I, TBO1_I, TBO2_J, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO1_I2, MyBO.SetBO2_J1, MyBO.SetBO2_J2));
        }

        [Test]
        public void TestNestedPlusBehindOrNav3()
        {
            RunTest(x => (x.BO1.I1 + x.BO1.I2) + x.BO2.J2 == null || (x.BO2.J1 + x.BO2.J2) + x.BO1.I2 == null,
                    Setters<TBO1_I, TBO2_J, TBO2_J, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO2_J2, MyBO.SetBO2_J1, MyBO.SetBO1_I2));
        }
        [Test]
        public void TestNestedPlusBehindNotOrNav3()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO2.J2 == null || (x.BO2.J1 + x.BO2.J2) + x.BO1.I2 == null),
                    Setters<TBO1_I, TBO2_J, TBO2_J, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO2_J2, MyBO.SetBO2_J1, MyBO.SetBO1_I2));
        }
        [Test]
        public void TestNestedPlusBehindNotAndNav3()
        {
            RunTest(x => !((x.BO1.I1 + x.BO1.I2) + x.BO2.J2 == null && (x.BO2.J1 + x.BO2.J2) + x.BO1.I2 == null),
                    Setters<TBO1_I, TBO2_J, TBO2_J, TBO1_I>(MyBO.SetBO1_I1, MyBO.SetBO2_J2, MyBO.SetBO2_J1, MyBO.SetBO1_I2));
        }
    }
}
