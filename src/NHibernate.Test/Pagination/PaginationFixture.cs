using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.Pagination
{
	[TestFixture]
	public class PaginationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"Pagination.DataPoint.hbm.xml"}; }
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.DefaultBatchFetchSize, "20");
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public void PagTest()
		{
			using(ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				for (int i = 0; i < 10; i++)
				{
					var dp = new DataPoint {X = (i * 0.1d)};
					dp.Y = Math.Cos(dp.X);
					s.Persist(dp);
				}
				t.Commit();
			}

			using(ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				int size =
					s.CreateSQLQuery("select Id, xval, yval, Description from DataPoint order by xval, yval").AddEntity(
						typeof (DataPoint)).SetMaxResults(5).List().Count;
				Assert.That(size, Is.EqualTo(5));
				size = s.CreateQuery("from DataPoint dp order by dp.X, dp.Y").SetFirstResult(5).SetMaxResults(2).List().Count;
				Assert.That(size, Is.EqualTo(2));
				size =
					s.CreateCriteria(typeof (DataPoint)).AddOrder(Order.Asc("X")).AddOrder(Order.Asc("Y")).SetFirstResult(8).List().
						Count;
				Assert.That(size, Is.EqualTo(2));
				size =
					s.CreateCriteria(typeof(DataPoint)).AddOrder(Order.Asc("X")).AddOrder(Order.Asc("Y")).SetMaxResults(10).SetFirstResult(8).List().
						Count;
				Assert.That(size, Is.EqualTo(2));
				t.Commit();
			}

			using(ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from DataPoint");
				t.Commit();
			}
		}

		[Test]
		public void PagingWithLock_NH2255()
		{
			if (Dialect is Oracle12cDialect)
				Assert.Ignore(@"Oracle does not support row_limiting_clause with for_update_clause
See: https://docs.oracle.com/database/121/SQLRF/statements_10002.htm#BABHFGAA");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new DataPoint() { X = 4 });
				s.Save(new DataPoint() { X = 5 });
				s.Save(new DataPoint() { X = 6 });
				s.Save(new DataPoint() { X = 7 });
				s.Save(new DataPoint() { X = 8 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var points =
					s.CreateCriteria<DataPoint>()
						.Add(Restrictions.Gt("X", 4.1d))
						.AddOrder(Order.Asc("X"))
						.SetLockMode(LockMode.Upgrade)
						.SetFirstResult(1)
						.SetMaxResults(2)
						.List<DataPoint>();

				Assert.That(points.Count, Is.EqualTo(2));
				Assert.That(points[0].X, Is.EqualTo(6d));
				Assert.That(points[1].X, Is.EqualTo(7d));
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from DataPoint").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}