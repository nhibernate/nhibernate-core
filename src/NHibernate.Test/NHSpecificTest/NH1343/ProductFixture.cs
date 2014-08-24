using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1343
{
	[TestFixture]
	public class ProductFixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from OrderLine");
				session.Delete("from Product");
				session.Flush();
			}
		}

		[Test]
		public void ProductQueryPassesParsingButFails()
		{
			Product product1 = new Product("product1");
			OrderLine orderLine = new OrderLine("1", product1);
			
			using (ISession session = OpenSession())
			{
				session.Save(product1);
				session.Save(orderLine);
				session.Flush();

				IQuery query = session.GetNamedQuery("GetLinesForProduct");
				query.SetParameter("product", product1);
				IList<OrderLine> list = query.List<OrderLine>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void ProductQueryPassesAndExecutesRightIfPuttingAlias()
		{
			Product product1 = new Product("product1");
			OrderLine orderLine = new OrderLine("1", product1);

			using (ISession session = OpenSession())
			{
				session.Save(product1);
				session.Save(orderLine);
				session.Flush();

				IQuery query = session.GetNamedQuery("GetLinesForProductWithAlias");
				query.SetParameter("product", product1);
				IList<OrderLine> list = query.List<OrderLine>();
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}