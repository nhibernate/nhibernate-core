using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
	}
}