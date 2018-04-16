using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1837
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
			using(ISession session=this.OpenSession())
			using(ITransaction tran=session.BeginTransaction())
			{
				var customer = new Customer {Name = "Fabio Maulo"};
				var order = new Order {Date = DateTime.Now, Customer = customer};
				customer.Orders.Add(order);
				session.Save(customer);
				session.Save(order);
				tran.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = this.OpenSession())
			using (ITransaction tran = session.BeginTransaction())
			{
				session.Delete("from Order");
				session.Delete("from Customer");
				tran.Commit();
			}
		}
		[Test]
		public void ExecutesOneQueryWithUniqueResultWithChildCriteriaNonGeneric()
		{
	
			Sfi.Statistics.Clear();
			using (ISession session = this.OpenSession())
			{
				var criteria = session.CreateCriteria(typeof(Order),"o");
				criteria.CreateCriteria("o.Customer", "c")
					.Add(Restrictions.Eq("c.Id", 1))
					.SetProjection(Projections.RowCount())
					.UniqueResult();
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			}
		}
		[Test]
		public void ExecutesOneQueryWithUniqueResultWithChildCriteriaGeneric()
		{
			Sfi.Statistics.Clear();
			using (ISession session = this.OpenSession())
			{
				session.CreateCriteria(typeof (Order), "o")
					.CreateCriteria("o.Customer", "c")
					.Add(Restrictions.Eq("c.Id", 1))
					.SetProjection(Projections.RowCount())
					.UniqueResult<int>();
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			}
		}
		[Test]
		public void ExecutesOneQueryWithUniqueResultWithCriteriaNonGeneric()
		{
			Sfi.Statistics.Clear();
			using (ISession session = this.OpenSession())
			{
				session.CreateCriteria(typeof (Order), "o")
					.SetProjection(Projections.RowCount())
					.UniqueResult();
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void ExecutesOneQueryWithUniqueResultWithCriteriaGeneric()
		{
			Sfi.Statistics.Clear();
			using (ISession session = this.OpenSession())
			{
				session.CreateCriteria(typeof (Order), "o")
					.SetProjection(Projections.RowCount())
					.UniqueResult<int>();
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			}
		}
	}
}
