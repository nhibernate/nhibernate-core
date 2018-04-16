using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2412
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from Order");
				s.Delete("from Customer");
				s.Flush();
			}
		}

		[Test]
		public void OrderByUsesLeftJoin()
		{
			ISession s = OpenSession();
			try
			{
				Customer c1 = new Customer {Name = "Allen"};
				s.Save(c1);
				Customer c2 = new Customer {Name = "Bob"};
				s.Save(c2);
				Customer c3 = new Customer {Name = "Charlie"};
				s.Save(c3);

				s.Save(new Order {Customer = c1});
				s.Save(new Order {Customer = c3});
				s.Save(new Order {Customer = c2});
				s.Save(new Order());

				s.Flush();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			try
			{
				var orders = s.Query<Order>().OrderBy(o => o.Customer.Name).ToList();
				Assert.AreEqual(4, orders.Count);
				if (orders[0].Customer == null)
				{
					CollectionAssert.AreEqual(new[] {"Allen", "Bob", "Charlie"}, orders.Skip(1).Select(o => o.Customer.Name).ToArray());
				}
				else
				{
					CollectionAssert.AreEqual(new[] { "Allen", "Bob", "Charlie" }, orders.Take(3).Select(o => o.Customer.Name).ToArray());
				}
			}
			finally
			{
				s.Close();
			}
		}
	}
}
