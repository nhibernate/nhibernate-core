using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH555
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (ISession s = OpenSession())
			{
				Customer c = new Customer();
				c.Name = "TestCustomer";
				s.Save(c);

				Article art = new Article();
				art.Name = "TheArticle1";
				art.Price = 10.5M;

				s.Save(art);

				Order o = c.CreateNewOrder();

				OrderLine ol = o.CreateNewOrderLine();
				ol.SetArticle(art);
				ol.NumberOfItems = 5;

				o.AddOrderLine(ol);

				s.Save(o);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				string hql = "select sum (ol.ArticlePrice * ol.NumberOfItems) " +
				             "from Order o, OrderLine ol, Customer c " +
				             "where c.Id = :custId and o.OrderDate >= :orderDate";

				IQuery q = s.CreateQuery(hql);
				q.SetInt32("custId", 1);
				q.SetDateTime("orderDate", DateTime.Now.AddMonths(-3));

				Assert.AreEqual(52.5m, (decimal) q.UniqueResult());
			}

			using (ISession s = OpenSession())
			{
				Order o = (Order) s.CreateQuery("from Order").UniqueResult();
				OrderLine ol = (OrderLine) o.OrderLines[0];
				s.Delete(ol);
				o.OrderLines.RemoveAt(0);
				s.Delete(o);
				s.Delete(o.OwningCustomer);
				s.Delete("from Article");
				s.Flush();
			}
		}
	}
}