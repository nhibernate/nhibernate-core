using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1716
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
		public void TimeSpanLargerThan24h()
		{
			var time = new TimeSpan(2, 2, 1, 0);
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
		public void TimeSpanNegative()
		{
			TimeSpan time = TimeSpan.FromDays(-1);
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