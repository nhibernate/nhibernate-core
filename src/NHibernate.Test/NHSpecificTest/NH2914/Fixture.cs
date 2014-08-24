using System;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2914
{
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Oracle8iDialect;
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var entity = new Entity { CreationTime = DateTime.Now };

				session.Save(entity);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Entity");
				tx.Commit();
			}
		}

		[Test]
		public void Linq_DateTimeDotYear_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Year == DateTime.Today.Year)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotMonth_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Month == DateTime.Today.Month)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotDay_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Day == DateTime.Today.Day)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotHour_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Hour <= 24)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotMinute_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Minute <= 60)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotSecond_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Second <= 60)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void Linq_DateTimeDotDate_WorksInOracle()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(x => x.CreationTime.Date == DateTime.Today)
					.ToList();

				tx.Commit();

				Assert.AreEqual(1, result.Count);
			}
		}
	}
}
