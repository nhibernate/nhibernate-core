using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    public class MassTestingNotAndDeMorganFixture : AbstractMassTestingFixture
    {
        protected override void TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
        {
            var result = session.Query<MyBO>().Where(condition);
            AreEqual(expectedIds, result.Select(bo => bo.Id).ToArray());
        }

        [Test]
        public void Test_NotUnequalIsNotTheSameAsEqual()
        {
            RunTest(x => !(x.BO1.I1 != 1),
                    Setters<TBO1_I>(MyBO.SetBO1_I1));
            // ... is the same as ...
            RunTest(x => !(x.BO1.I1 != 1),
                    Setters<TBO1_I>(MyBO.SetBO1_I1));
            // ... is NOT the same as ...
            RunTest(x => x.BO1.I1 == 1,
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
            // The latter returns fewer results! - namely NOT the ones where BO1 is null.
        }

        [Test]
        public void Test_NotEqualIsNotTheSameAsNotequal()
        {
            RunTest(x => !(x.BO1.I1 == 1),
                    Setters<TBO1_I>(MyBO.SetBO1_I1));
            // ... is the same as ...
            RunTest(x => !(x.BO1.I1 == 1),
                    Setters<TBO1_I>(MyBO.SetBO1_I1));
            // ... is NOT the same as ...
            RunTest(x => x.BO1.I1 != 1,
                    Setters<TBO1_I>(MyBO.SetBO1_I1));
            // As before, the latter returns fewer results! - namely NOT the ones where BO1 is null.
        }
        
        [Test]
        public void Test_DeMorganNotAnd()
        {
            //      BO1.I1  BO2.J1      x.BO1.I1 != 1   x.BO2.J1 != 1   &&  !   Result (3v-->2v)    Linq2Obj
            //      null    null            n               n           n   n   f                   
            //      null    0               n               t           n   n   f                   
            //      null    1               n               f           f   t   t                   
            //      0       null            t               n           n   n   f                   
            //      0       0               t               t           t   f   f                   f
            //      0       1               t               f           f   t   t                   t
            //      1       null            f               n           f   t   t                   
            //      1       0               f               t           f   t   t                   t
            //      1       1               f               f           f   t   t                   t

            RunTest(x => !(x.BO1.I1 != 1 && x.BO2.J1 != 1),
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
        }

        [Test]
        public void Test_DeMorganNotOr()
        {
            RunTest(x => !(x.BO1.I1 != 1 || x.BO2.J1 != 1),
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
            RunTest(x => !(x.BO1.I1 != 1 || x.BO2.J1 != 1),
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
            RunTest(x => !(x.BO1.I1 != 1) && !(x.BO2.J1 != 1),
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
            // ... is NOT the same as ...
            RunTest(x => x.BO1.I1 == 1 && x.BO2.J1 == 1,
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
        }

        [Test]
        public void Test_NotNotCanBeEliminated()
        {
            RunTest(x => !(!(x.BO1.I1 != 1 && x.BO2.J1 != 1)),
                    Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
        }
    }
}
