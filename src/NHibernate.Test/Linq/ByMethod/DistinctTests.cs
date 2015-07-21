using System;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class DistinctTests : LinqTestCase
	{
		public class OrderDto
		{
			public string ShipCountry { get; set; }
			public DateTime? ShippingDate { get; set; }
			public DateTime? OrderDate { get; set; }
		}

		private static T Transform<T>(T value)
		{
			return value;
		}

		[Test]
		public void DistinctOnAnonymousTypeProjection()
		{
			//NH-2380
			var result = db.Orders
				.Select(x => new {x.ShippingDate})
				.Distinct()
				.ToArray();

			Assert.That(result.Length, Is.EqualTo(388));
		}

		[Test]
		public void DistinctOnComplexAnonymousTypeProjection()
		{
			//NH-2380
			var result = db.Orders
				.Select(x => new
								 {
									 x.ShippingDate,
									 x.OrderDate
								 })
				.Distinct()
				.ToArray();

			Assert.That(result.Length, Is.EqualTo(774));
		}

		[Test]
		public void DistinctOnTypeProjection()
		{
			//NH-2486
			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
								 {
									 ShippingDate = x.ShippingDate
								 })
				.Distinct()
				.ToArray();

			Assert.That(result.Length, Is.EqualTo(388));
		}

		[Test]
		public void DistinctOnTypeProjectionTwoProperty()
		{
			//NH-2486
			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
								 {
									 ShippingDate = x.ShippingDate,
									 OrderDate = x.OrderDate
								 })
				.Distinct()
				.ToArray();

			Assert.That(result.Length, Is.EqualTo(774));
		}

		[Test]
		public void DistinctOnTypeProjectionWithHqlMethodIsOk()
		{
			// Sort of related to NH-2645.

			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
				{
					ShipCountry = x.ShippingAddress.Country.ToLower(),     // Should be translated to HQL/SQL.
					ShippingDate = x.ShippingDate,
					OrderDate = x.OrderDate.Value.Date,
				})
				.Distinct()
				.ToArray();

			Assert.That(result.Length, Is.EqualTo(824));
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Cannot use distinct on result that depends on methods for which no SQL equivalent exist.")]
		public void DistinctOnTypeProjectionWithCustomProjectionMethodsIsBlocked1()
		{
			// Sort of related to NH-2645.

			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
								 {
									 ShippingDate = Transform(x.ShippingDate),
									 OrderDate = Transform(x.OrderDate)
								 })
				.Distinct()
				.ToArray();
		}


		[Test]
		[ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Cannot use distinct on result that depends on methods for which no SQL equivalent exist.")]
		public void DistinctOnTypeProjectionWithCustomProjectionMethodsIsBlocked2()
		{
			// Sort of related to NH-2645.

			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
				{
					ShippingDate = x.ShippingDate,
					OrderDate = x.OrderDate.Value.AddMonths(5),  // As of 2012-01-25, AddMonths() is executed locally.
				})
				.Distinct()
				.ToArray();
		}
	}
}