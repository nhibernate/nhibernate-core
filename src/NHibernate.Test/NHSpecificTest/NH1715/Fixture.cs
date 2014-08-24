using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1715
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public void TimeSpanLargerThan2h()
		{
			var time = new TimeSpan(0, 2, 1, 0);
			var entity = new ClassA {Time = time};
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				Assert.AreEqual(time, s.Get<ClassA>(entity.Id).Time);
			}
		}

		[Test]
		public void VerifyDaysShouldBeZeroInSmallTimeSpan()
		{
			var time = new TimeSpan(1, 0, 0);
			var entity = new ClassA {Time = time};
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				Assert.AreEqual(0, s.Get<ClassA>(entity.Id).Time.Days);
			}
		}
	}
}