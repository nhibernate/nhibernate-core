using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.DomainModel.Northwind.Entities;
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
		public void OrdersOrderLinesId()
		{
			var orders = db.Orders
				.Select(o => new
				{
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
		public void TimesheetIdAndUserLastLoginDates()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							Users = o.Users.Select(x => x.LastLoginDate).ToArray()
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test]
		public void TimesheetIdAndUserLastLoginDatesAndEntriesIds()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							LastLoginDates = o.Users.Select(u => u.LastLoginDate).ToArray(),
							EntriesIds = o.Entries.Select(e => e.Id).ToArray()
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].LastLoginDates, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetIdAndUsersTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							Users = o.Users.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetAndUsersTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o,
							Users = o.Users.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetUsersTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							Users = o.Users.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetIdAndUsersAndEntriesTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							Users = o.Users.Select(x => x),
							Entries = o.Entries.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetAndUsersAndEntriesTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o,
							Users = o.Users.Select(x => x),
							Entries = o.Entries.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-2986")]
		public void TimesheetUsersAndEntriesTransparentProjection()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							Users = o.Users.Select(x => x),
							Entries = o.Entries.Select(x => x)
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetIdAndUsers()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							o.Users
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetAndUsers()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o,
							o.Users
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetUsers()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Users
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetIdAndUsersAndEntries()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Id,
							o.Users,
							o.Entries
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetAndUsersAndEntries()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o,
							o.Users,
							o.Entries
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
		}

		[Test(Description = "NH-3333")]
		public void TimesheetUsersAndEntries()
		{
			var timesheets = db.Timesheets
				.Select(o =>
						new
						{
							o.Users,
							o.Entries
						})
				.ToList();

			Assert.That(timesheets.Count, Is.EqualTo(3));
			Assert.That(timesheets[0].Users, Is.Not.Empty);
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

		[Test]
		public void NoNestedSelects_AnyOnGroupBySubquery()
		{
			var subQuery = from vms in db.Animals
						   group vms by vms.Father
							into vmReqs
						   select vmReqs.Max(x => x.Id);

			var outerQuery = from vm in db.Animals
							 where subQuery.Any(x => vm.Id == x)
							 select vm;
			var animals = outerQuery.ToList();
			Assert.That(animals.Count, Is.EqualTo(2));
		}

		//NH-3155
		[Test]
		public void NoNestedSelects_ContainsOnGroupBySubquery()
		{
			var subQuery = from vms in db.Animals
						   where vms.BodyWeight > 0
						   group vms by vms.Father
							into vmReqs
						   select vmReqs.Max(x => x.Id);

			var outerQuery = from vm in db.Animals
							 where subQuery.Contains(vm.Id)
							 select vm;

			var animals = outerQuery.ToList();
			Assert.That(animals.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanSelectWithWhereSubQuery()
		{
			var query = from timesheet in db.Timesheets
						select new
						{
							timesheet.Id,
							Entries = timesheet.Entries.Where(e => e.NumberOfHours >= 0).ToList()
						};

			var list = query.ToList();

			Assert.AreEqual(3, list.Count);
		}

		[Test(Description = "GH-2540")]
		public void CanSelectWithAsQueryableAndWhereSubQuery()
		{
			var query = from timesheet in db.Timesheets
						select new
						{
							timesheet.Id,
							Entries = timesheet.Entries.AsQueryable().Where(e => e.NumberOfHours >= 0).ToList()
						};

			var list = query.ToList();

			Assert.AreEqual(3, list.Count);
		}

		[Test(Description = "GH-2540")]
		public void CanSelectWithAsQueryableAndWhereSubQueryToArray()
		{
			var query = from timesheet in db.Timesheets
						select new
						{
							timesheet.Id,
							Entries = timesheet.Entries.AsQueryable().Where(e => e.NumberOfHours >= 0).ToArray()
						};

			var list = query.ToList();

			Assert.AreEqual(3, list.Count);
		}

		[Test(Description = "GH-2540")]
		public void CanSelectWithAsQueryableAndWhereSubQueryWithExternalPredicate()
		{
			Expression<Func<TimesheetEntry, bool>> predicate = e => e.NumberOfHours >= 0;

			var query = from timesheet in db.Timesheets
						select new
						{
							timesheet.Id,
							Entries = timesheet.Entries.AsQueryable().Where(predicate).ToList()
						};

			var list = query.ToList();

			Assert.AreEqual(3, list.Count);
		}
	}
}
