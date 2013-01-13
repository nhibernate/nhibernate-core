using System.Linq;
using NHibernate.Cfg;
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

	}
}
