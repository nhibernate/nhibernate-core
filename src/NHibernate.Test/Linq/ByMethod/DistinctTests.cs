using System;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class DistinctTests : LinqTestCase
	{
		public class OrderDto
		{
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

			result.Length.Should().Be.EqualTo(388);
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

			result.Length.Should().Be.EqualTo(774);
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

			result.Length.Should().Be.EqualTo(388);
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

			result.Length.Should().Be.EqualTo(774);
		}

		[Test]
		public void DistinctOnTypeProjectionWithCustomProjectionMethods()
		{
			//NH-2645
			OrderDto[] result = db.Orders
				.Select(x => new OrderDto
								 {
									 ShippingDate = Transform(x.ShippingDate),
									 OrderDate = Transform(x.OrderDate)
								 })
				.Distinct()
				.ToArray();

			result.Length.Should().Be.EqualTo(774);
		}
	}
}