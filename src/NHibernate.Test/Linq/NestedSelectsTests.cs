using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class NestedSelectsTests : LinqTestCase
	{
		[Test]
		public void OrdersIdWithOrderLinesId()
		{
			var orders = db.Orders
				.Select(o => new
								 {
									 o.OrderId,
									 OrderLinesIds = o.OrderLines.Select(ol => ol.Id).ToArray()
								 })
				.ToList();

			Assert.That(orders.Count, Is.EqualTo(830));
		}

		[Test]
		public void OrdersIdWithOrderLinesIdShouldBeNotLazy()
		{
			var orders = db.Orders
				.Select(o => new
								 {
									 o.OrderId,
									 OrderLinesIds = o.OrderLines.Select(ol => ol.Id)
								 })
				.ToList();

			Assert.That(orders.Count, Is.EqualTo(830));
			Assert.That(orders[0].OrderLinesIds, Is.InstanceOf<ReadOnlyCollection<long>>());
		}

		[Test]
		public void OrdersIdWithOrderLinesIdToArray()
		{
			var orders = db.Orders
				.Select(o => new
								 {
									 o.OrderId,
									 OrderLinesIds = o.OrderLines.Select(ol => ol.Id).ToArray()
								 })
				.ToArray();

			Assert.That(orders.Length, Is.EqualTo(830));
		}

		[Test]
		public void OrdersIdWithOrderLinesIdAndDiscount()
		{
			var orders = db.Orders
				.Select(o =>
						new
							{
								o.OrderId,
								OrderLines = o.OrderLines.Select(ol =>
																 new
																	 {
																		 ol.Id,
																		 ol.Discount
																	 }).ToArray()
							})
				.ToList();

			Assert.That(orders.Count, Is.EqualTo(830));
		}

		[Test]
		public void OrdersIdAndDateWithOrderLinesIdAndDiscount()
		{
			var orders = db.Orders
				.Select(o =>
						new
							{
								o.OrderId,
								o.OrderDate,
								OrderLines = o.OrderLines.Select(ol =>
																 new
																	 {
																		 ol.Id,
																		 ol.Discount
																	 }).ToArray()
							})
				.ToList();

			Assert.That(orders.Count, Is.EqualTo(830));
		}
		
		[Test]
		public void CategoriesIdAndDateWithOrderLinesIdAndDiscount()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
							{
								o.Id,
								LastLoginDates = o.Users.Select(x => x.LastLoginDate).ToArray()
							})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].LastLoginDates, Is.Not.Empty);
		}

		[Test]
		public void EmployeesIdAndWithSubordinatesId()
		{
			var emplyees = db.Employees
				.Select(o =>
						new
							{
								o.EmployeeId,
								SubordinatesIds = o.Subordinates.Select(so => so.EmployeeId).ToArray()
							})
				.ToList();

			Assert.That(emplyees.Count, Is.EqualTo(9));
		}

		[Test]
		public void OrdersIdWithOrderLinesNestedWhereId()
		{
			var orders = db.Orders
				.Select(o => new
								 {
									 o.OrderId,
									 OrderLinesIds = o.OrderLines
										.Where(ol => ol.Discount > 1)
										.Select(ol => ol.Id)
										.ToArray()
								 })
				.ToList();

			Assert.That(orders.Count, Is.EqualTo(830));
			Assert.That(orders[0].OrderLinesIds, Is.Empty);
		}
	}
}