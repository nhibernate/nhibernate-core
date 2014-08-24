using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class SumTests : LinqTestCase
	{
		[Test]
		[ExpectedException]
		public void EmptySumDecimal()
		{
			db.OrderLines.Where(ol => false).Sum(ol => ol.Discount);
		}

		[Test]
		public void EmptySumCastNullableDecimal()
		{
			decimal total = db.OrderLines.Where(ol => false).Sum(ol => (decimal?)ol.Discount) ?? 0;
			Assert.AreEqual(0, total);
		}

		[Test]
		public void SumDecimal()
		{
			decimal total = db.OrderLines.Sum(ol => ol.Discount);
			Assert.Greater(total, 0);
		}

		[Test]
		public void EmptySumNullableDecimal()
		{
			decimal total = db.Orders.Where(ol => false).Sum(ol => ol.Freight) ?? 0;
			Assert.AreEqual(0, total);
		}

		[Test]
		public void SumNullableDecimal()
		{
			decimal? total = db.Orders.Sum(ol => ol.Freight);
			Assert.Greater(total, 0);
		}

		[Test]
		public void SumSingle()
		{
			float total = db.Products.Sum(p => p.ShippingWeight);
			Assert.Greater(total, 0);
		}
	}
}