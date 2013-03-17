using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class GetValueOrDefaultTests : LinqTestCase
	{
		[Test]
		public void CoalesceInWhere()
		{
			var orders = db.Orders
						   .Where(x => (x.Freight ?? 100) > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}

		[Test]
		public void GetValueOrDefaultInWhere()
		{
			var orders = db.Orders
						   .Where(x => x.Freight.GetValueOrDefault(100) > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}

		[Test]
		public void GetValueOrDefaultWithSingleArgumentInWhere()
		{
			var orders = db.Orders
						   .Where(x => x.Freight.GetValueOrDefault() > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}
	}
}
