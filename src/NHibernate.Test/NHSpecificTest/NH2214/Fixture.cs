﻿using System.Linq;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2214
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect || dialect is MsSql2008Dialect;
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				session.Save(new DomainClass {Id = 1, Name = "Name"});
				session.Save(new DomainClass {Id = 2, Name = "Name"});
				session.Save(new DomainClass {Id = 3, Name = "Name1"});
				session.Save(new DomainClass {Id = 4, Name = "Name1"});
				session.Save(new DomainClass {Id = 5, Name = "Name2"});
				session.Save(new DomainClass {Id = 6, Name = "Name2"});
				session.Save(new DomainClass {Id = 7, Name = "Name3"});
				session.Save(new DomainClass {Id = 8, Name = "Name3"});
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from DomainClass");
				session.Flush();
			}
		}

		[Test]
		public void PagedQueryWithDistinct()
		{
			using (ISession session = OpenSession())
			{
				const int page = 2;
				const int rows = 2;

				var criteria = DetachedCriteria
					.For<DomainClass>("d")
					.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("Name"))))
					.SetFirstResult((page - 1)*rows)
					.SetMaxResults(rows)
					.AddOrder(Order.Asc("Name"));

				var query = criteria.GetExecutableCriteria(session);
				var result = query.List();

				Assert.AreEqual("Name2", result[0]);
				Assert.AreEqual("Name3", result[1]);
			}
		}

		[Test]
		public void PagedQueryWithDistinctAndOrderingByNonProjectedColumn()
		{
			using (ISession session = OpenSession())
			{
				const int page = 2;
				const int rows = 2;

				var criteria = DetachedCriteria
					.For<DomainClass>("d")
					.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("Name"))))
					.SetFirstResult((page - 1)*rows)
					.SetMaxResults(rows)
					.AddOrder(Order.Asc("Id"));

				var query = criteria.GetExecutableCriteria(session);

				Assert.Throws<HibernateException>(() => query.List());
			}
		}

		[Test]
		public void PagedLinqQueryWithDistinct()
		{
			using (ISession session = OpenSession())
			{
				const int page = 2;
				const int rows = 2;

				var query = (from t in session.Query<DomainClass>()
				             orderby t.Name
				             select t.Name).Distinct().Skip((page - 1)*rows).Take(rows);

				var result = query.ToList();

				Assert.AreEqual("Name2", result[0]);
				Assert.AreEqual("Name3", result[1]);
			}
		}
	}
}