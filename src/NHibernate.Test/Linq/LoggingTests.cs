using System.Linq;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class LoggingTests : LinqTestCase
	{
		[Test]
		public void PageBetweenProjections()
		{
			using (var spy = new LogSpy("NHibernate.Linq"))
			{
				var subquery = db.Products.Where(p => p.ProductId > 5);

				var list = db.Products.Where(p => subquery.Contains(p))
				             .Skip(5).Take(10)
				             .ToList();

				var logtext = spy.GetWholeLog();

				const string expected =
					"Expression (partially evaluated): value(NHibernate.Linq.NhQueryable`1[NHibernate.DomainModel.Northwind.Entities.Product]).Where(p => value(NHibernate.Linq.NhQueryable`1[NHibernate.DomainModel.Northwind.Entities.Product]).Where(p => (p.ProductId > 5)).Contains(p)).Skip(5).Take(10)";
				Assert.That(logtext, Is.StringContaining(expected));
			}
		}


		[Test]
		public void CanLogLinqExpressionWithoutInitializingContainedProxy()
		{
			var productId = db.Products.Select(p => p.ProductId).First();

			using (var logspy = new LogSpy("NHibernate.Linq"))
			{
				var productProxy = session.Load<Product>(productId);
				Assert.That(NHibernateUtil.IsInitialized(productProxy), Is.False);

				var result = from product in db.Products
				             where product == productProxy
				             select product;

				Assert.That(result.Count(), Is.EqualTo(1));

				// Verify that the expected logging did happen.
				var actualLog = logspy.GetWholeLog();

				const string expectedLog =
					"Expression (partially evaluated): value(NHibernate.Linq.NhQueryable`1[NHibernate.DomainModel.Northwind.Entities.Product])" + 
					".Where(product => (product == Product#1)).Count()";
				Assert.That(actualLog, Is.StringContaining(expectedLog));

				// And verify that the proxy in the expression wasn't initialized.
				Assert.That(NHibernateUtil.IsInitialized(productProxy), Is.False,
				            "ERROR: We expected the proxy to NOT be initialized.");
			}
		}
	}
}
