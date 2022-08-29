using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
	[TestFixture]
	public class MassTestingNotAndDeMorganFixture : AbstractMassTestingFixture
	{
		protected override int TestAndAssert(Expression<Func<MyBO, bool>> condition, ISession session, IEnumerable<int> expectedIds)
		{
			var result = session.Query<MyBO>().Where(condition);
			AreEqual(expectedIds, result.Select(bo => bo.Id).ToArray());
			return expectedIds.Count();
		}

		[Test]
		public void Test_NotUnequalIsTheSameAsEqual()
		{
			int r1 = RunTest(x => !(x.BO1.I1 != 1),
							 Setters<TBO1_I>(MyBO.SetBO1_I1));
			int r2 = RunTest(x => x.BO1.I1 == 1,
							 Setters<TBO1_I>(MyBO.SetBO1_I1));
			Assert.AreEqual(r1, r2);
			Assert.Greater(r1, 0);

			r1 = RunTest(x => !(x.BO1.I1 != 1),
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			r2 = RunTest(x => x.BO1.I1 == 1,
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			Assert.AreEqual(r1, r2);
			Assert.Greater(r1, 0);
		}

		[Test]
		public void Test_NotEqualIsTheSameAsNotequal()
		{
			// Already the following yields different results for I1 == null even though
			// it does NOT throw an exception in Linq2Objects:
			//      ... RunTest(x => x.BO1.I1 != 1, ...);
			// * In C# logic, we get null != 1 <=> true
			// * In SQL logic, we get null != 1 <=> logical-null => false

			// To exclude this case, we can either make it false in C# ...
			int r1 = RunTest(x => x.BO1.I1 != null && x.BO1.I1 != 1,
							 Setters<TBO1_I>(MyBO.SetBO1_I1));
			// ... or force it to true in SQL
			int r2 = RunTest(x => x.BO1.I1 == null || x.BO1.I1 != 1,
							 Setters<TBO1_I>(MyBO.SetBO1_I1));

			// Also the following condition yields different results for I1 == null even 
			// though it does NOT throw an exception in Linq2Objects:
			//      ... RunTest(x => !(x.BO1.I1 == 1), ...);
			// * In C# logic, we get !(null == 1) <=> !(false) <=> true
			// * In SQL logic, we get !(null == 1) <=> !(logical-null) <=> logical-null => false

			// Again, to exclude this case, we can either make the inner part true in C# ...
			int r3 = RunTest(x => !(x.BO1.I1 == null || x.BO1.I1 == 1),
							 Setters<TBO1_I>(MyBO.SetBO1_I1));
			// ... or force it to false in SQL:
			int r4 = RunTest(x => !(x.BO1.I1 != null && x.BO1.I1 == 1),
							 Setters<TBO1_I>(MyBO.SetBO1_I1));

			Assert.Greater(r1, 0);
			Assert.Greater(r2, 0);

			// We also expect the !(==) versions to return the same result as the != versions.
			Assert.AreEqual(r1, r3);
			Assert.AreEqual(r2, r4);
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
			int r1 = RunTest(x => !(x.BO1.I1 != 1 || x.BO2.J1 != 1),
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			int r2 = RunTest(x => !(x.BO1.I1 != 1) && !(x.BO2.J1 != 1),
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			int r3 = RunTest(x => x.BO1.I1 == 1 && x.BO2.J1 == 1,
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			Assert.AreEqual(r1, r2);
			Assert.AreEqual(r2, r3);
			Assert.Greater(r1, 0);
		}

		[Test]
		public void Test_NotNotCanBeEliminated()
		{
			// The following condition does *not* return the same values if I1 and/or J1 are
			// null in Linq2Objects and in Nhib.Linq:
			//     x => x.BO1.I1 != 1 && x.BO2.J1 != 1,
			// First, assume I1 == null and J1 == 0:
			// * In C# (Linq2Objects), we get null != 1 && 0 != 1 <=> true && true <=> true
			// * In SQL (NHib.Linq), we get null != 1 && <=> logical-null && true <=> logical-null => false
			// For I1 == 0 and J1 == null we get the same problem, as the condition is symmetric.

			// To repair this, we force "SQL" to true for nulls:
			int r1 = RunTest(x => (x.BO1.I1 == null || x.BO1.I1 != 1) && (x.BO2.J1 == null || x.BO2.J1 != 1),
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			int r2 = RunTest(x => !!((x.BO1.I1 == null || x.BO1.I1 != 1) && (x.BO2.J1 == null || x.BO2.J1 != 1)),
							 Setters<TBO1_I, TBO2_J>(MyBO.SetBO1_I1, MyBO.SetBO2_J1));
			Assert.Greater(r1, 0);
			Assert.AreEqual(r1, r2);
		}
	}
}
