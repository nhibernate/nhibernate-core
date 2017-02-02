using System;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class CountTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		[Test]
		public void CountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.Count();

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public void CountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Count();

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public void Count_ReturnsNumberOfRecords()
		{
			//NH-2722
			var result = db.Orders
				.Count();

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public void LongCountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.LongCount();

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public void LongCountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.LongCount();

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public void LongCount_ReturnsNumberOfRecords()
		{
			//NH-2722
			var result = db.Orders
				.LongCount();

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public void CountOnJoinedGroupBy()
		{
			//NH-3001
			var query = from o in db.Orders
						join ol in db.OrderLines
						on o equals ol.Order
						group ol by ol.Product.ProductId
							into temp
							select new { temp.Key, count = temp.Count() };

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(77));
		}
	}
}
