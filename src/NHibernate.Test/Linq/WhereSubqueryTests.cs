using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class WhereSubqueryTests : LinqTestCase
	{
		[Test]
		public void TimesheetsWithNoEntries()
		{
			var query = (from timesheet in db.Timesheets
						 where !timesheet.Entries.Any()
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithCountSubquery()
		{
// ReSharper disable UseMethodAny.1
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Count() >= 1
						 select timesheet).ToList();
// ReSharper restore UseMethodAny.1

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithCountSubqueryReversed()
		{
// ReSharper disable UseMethodAny.1
			var query = (from timesheet in db.Timesheets
						 where 1 <= timesheet.Entries.Count()
						 select timesheet).ToList();
// ReSharper restore UseMethodAny.1

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithCountSubqueryComparedToProperty()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Count() > timesheet.Id
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithCountSubqueryComparedToPropertyReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Id < timesheet.Entries.Count()
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithAverageSubquery()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Average(e => e.NumberOfHours) > 12
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithAverageSubqueryReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where 12 < timesheet.Entries.Average(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		[Ignore("Need to coalesce the subquery - timesheet with no entries should return average of 0, not null")]
		public void TimeSheetsWithAverageSubqueryComparedToProperty()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Average(e => e.NumberOfHours) < timesheet.Id
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		[Ignore("Need to coalesce the subquery - timesheet with no entries should return average of 0, not null")]
		public void TimeSheetsWithAverageSubqueryComparedToPropertyReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Id > timesheet.Entries.Average(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithMaxSubquery()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Max(e => e.NumberOfHours) == 14
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithMaxSubqueryReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where 14 == timesheet.Entries.Max(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithMaxSubqueryComparedToProperty()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Max(e => e.NumberOfHours) > timesheet.Id
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithMaxSubqueryComparedToPropertyReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Id < timesheet.Entries.Max(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithMinSubquery()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Min(e => e.NumberOfHours) < 7
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithMinSubqueryReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where 7 > timesheet.Entries.Min(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithMinSubqueryComparedToProperty()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Min(e => e.NumberOfHours) > timesheet.Id
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithMinSubqueryComparedToPropertyReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Id < timesheet.Entries.Min(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimeSheetsWithSumSubquery()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Sum(e => e.NumberOfHours) <= 20
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithSumSubqueryReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where 20 >= timesheet.Entries.Sum(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		[Ignore("Need to coalesce the subquery - timesheet with no entries should return sum of 0, not null")]
		public void TimeSheetsWithSumSubqueryComparedToProperty()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Sum(e => e.NumberOfHours) <= timesheet.Id
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		[Ignore("Need to coalesce the subquery - timesheet with no entries should return sum of 0, not null")]
		public void TimeSheetsWithSumSubqueryComparedToPropertyReversed()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Id >= timesheet.Entries.Sum(e => e.NumberOfHours)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimeSheetsWithStringContainsSubQuery()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.Any(e => e.Comments.Contains("testing"))
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NG-2998")]
		public void TimeSheetsWithStringContainsSubQueryWithAsQueryable()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.AsQueryable().Any(e => e.Comments.Contains("testing"))
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2998")]
		public void TimeSheetsWithStringContainsSubQueryWithAsQueryableAndExternalPredicate()
		{
			Expression<Func<TimesheetEntry, bool>> predicate = e => e.Comments.Contains("testing");

			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.AsQueryable().Any(predicate)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2998")]
		public void CategoriesSubQueryWithAsQueryableAndExternalPredicateWithClosure()
		{
			var ids = new[] { 1 };
			var quantities = new[] { 100 };

			Expression<Func<OrderLine, bool>> predicate2 = e => quantities.Contains(e.Quantity);
			Expression<Func<Product, bool>> predicate1 = e => !ids.Contains(e.ProductId)
															  && e.OrderLines.AsQueryable().Any(predicate2);

			var query = (from category in db.Categories
						 where category.Products.AsQueryable().Any(predicate1)
						 select category).ToList();

			Assert.That(query.Count, Is.EqualTo(6));
		}

		[Test(Description = "NH-2998")]
		public void TimeSheetsSubQueryWithAsQueryableAndExternalPredicateWithSecondLevelClosure()
		{
			var ids = new[] { 1 };

			Expression<Func<TimesheetEntry, bool>> predicate = e => !ids.Contains(e.Id);

			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.AsQueryable().Any(predicate)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2998")]
		public void TimeSheetsSubQueryWithAsQueryableAndExternalPredicateWithArray()
		{
			Expression<Func<TimesheetEntry, bool>> predicate = e => !new[] { 1 }.Contains(e.Id);

			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.AsQueryable().Any(predicate)
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2998")]
		public void TimeSheetsSubQueryWithAsQueryableWithArray()
		{
			var query = (from timesheet in db.Timesheets
						 where timesheet.Entries.AsQueryable().Any(e => !new[] { 1 }.Contains(e.Id))
						 select timesheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-3002")]
		public void HqlOrderLinesWithInnerJoinAndSubQuery()
		{
			var lines = session.CreateQuery(@"select c from OrderLine c
join c.Order o
where o.Customer.CustomerId = 'VINET'
	and not exists (from c.Order.Employee.Subordinates x where x.EmployeeId = 100)
").List<OrderLine>();

			Assert.That(lines.Count, Is.EqualTo(10));
		}

		[Test(Description = "NH-3002")]
		public void HqlOrderLinesWithImpliedJoinAndSubQuery()
		{
			var lines = session.CreateQuery(@"from OrderLine c
where c.Order.Customer.CustomerId = 'VINET'
	and not exists (from c.Order.Employee.Subordinates x where x.EmployeeId = 100)
").List<OrderLine>();

			Assert.That(lines.Count, Is.EqualTo(10));
		}

		[Test(Description = "NH-2999 and NH-2988")]
		public void OrderLinesWithImpliedJoinAndSubQuery()
		{
// ReSharper disable SimplifyLinqExpression
			var lines = (from l in db.OrderLines
						 where l.Order.Customer.CustomerId == "VINET"
						 where !l.Order.Employee.Subordinates.Any(x => x.EmployeeId == 100)
						 select l).ToList();
// ReSharper restore SimplifyLinqExpression

			Assert.That(lines.Count, Is.EqualTo(10));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery1()
		{
			var query = (from order in db.Orders
						 where order.OrderLines.Any()
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(830));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery2()
		{
			var subquery = from line in db.OrderLines
						   select line.Order;

			var query = (from order in db.Orders
						 where subquery.Contains(order)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(830));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery3()
		{
			var subquery = from line in db.OrderLines
						   select line.Order.OrderId;

			var query = (from order in db.Orders
						 where subquery.Contains(order.OrderId)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(830));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery4()
		{
			var subquery = from line in db.OrderLines
						   select line.Order;

			var query = (from order in db.Orders
						 where subquery.Any(x => x.OrderId == order.OrderId)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(830));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery5()
		{
			var query = (from order in db.Orders
						 where order.OrderLines.Any(x => x.Quantity == 5)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(61));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery6()
		{
			var subquery = from line in db.OrderLines
						   where line.Quantity == 5
						   select line.Order;

			var query = (from order in db.Orders
						 where subquery.Contains(order)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(61));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery7()
		{
			var subquery = from line in db.OrderLines
						   where line.Quantity == 5
						   select line.Order.OrderId;

			var query = (from order in db.Orders
						 where subquery.Contains(order.OrderId)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(61));
		}

		[Test(Description = "NH-2904")]
		public void OrdersWithSubquery8()
		{
			var subquery = from line in db.OrderLines
						   where line.Quantity == 5
						   select line.Order;

			var query = (from order in db.Orders
						 where subquery.Any(x => x.OrderId == order.OrderId)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(61));
		}

		[Test(Description = "NH-2654")]
		public void CategoriesWithDiscountedProducts()
		{
			var query = (from c in db.Categories
						 where c.Products.Any(p => p.Discontinued)
						 select c).ToList();

			Assert.That(query.Count, Is.EqualTo(5));
		}

		[Test(Description = "NH-3147")]
		public void OrdersWithSubqueryWithJoin()
		{
			var subquery = from line in db.OrderLines
						   join product in db.Products
							   on line.Product.ProductId equals product.ProductId
						   where line.Quantity == 5
						   select line.Order;

			var query = (from order in db.Orders
						 where subquery.Contains(order)
						 select order).ToList();

			Assert.That(query.Count, Is.EqualTo(61));
		}

		[Test(Description = "NH-2899")]
		public void ProductsWithSubquery()
		{
			var result = (from p in db.Products
						  where (from c in db.Categories
								 where c.Name == "Confections"
								 select c).Contains(p.Category)
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}

		[Test(Description = "NH-2762")]
		public void ProductsWithSubqueryAsIEnumerable()
		{
// ReSharper disable RedundantEnumerableCastCall
			var categories = (from c in db.Categories
							  where c.Name == "Confections"
							  select c).ToList().OfType<ProductCategory>();
// ReSharper restore RedundantEnumerableCastCall

			var result = (from p in db.Products
						  where categories.Contains(p.Category)
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}

		[Test(Description = "NH-2762")]
		public void ProductsWithSubqueryAsIGrouping()
		{
			var categories = (from c in db.Categories
							  where c.Name == "Confections"
							  select c).ToLookup(c => c.Name).Single();

			var result = (from p in db.Products
						  where categories.Contains(p.Category)
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}

		[Test(Description = "NH-3155"), Ignore("Not fixed yet.")]
		public void SubqueryWithGroupBy()
		{
			var sq = db.Orders
				.GroupBy(x => x.ShippingDate)
				.Select(x => x.Max(o => o.OrderId));

			var result = db.Orders
				.Where(x => sq.Contains(x.OrderId))
				.Select(x => x.OrderId)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(388));
		}

		[Test(Description = "NH-3111")]
		public void SubqueryWhereFailingTest()
		{
			var list = (db.OrderLines
				.Select(ol => new
				{
					ol.Discount,
					ShipperPhoneNumber = db.Shippers
						.Where(sh => sh.ShipperId == ol.Order.Shipper.ShipperId)
						.Select(sh => sh.PhoneNumber)
						.FirstOrDefault()
				})).ToList();

			Assert.That(list.Count, Is.EqualTo(2155));
		}

		[Test(Description = "NH-3111")]
		public void SubqueryWhereFailingTest2()
		{
			var list = db.OrderLines
				.Select(ol => new
				{
					ol.Discount,
					ShipperPhoneNumber = db.Shippers
						.Where(sh => sh == ol.Order.Shipper)
						.Select(sh => sh.PhoneNumber)
						.FirstOrDefault()
				}).ToList();

			Assert.That(list.Count, Is.EqualTo(2155));
		}

		[Test(Description = "NH-3111")]
		public void SubqueryWhereFailingTest3()
		{
			var list = db.OrderLines
				.Select(ol => new
				{
					ol.Discount,
					ShipperPhoneNumber = db.Orders
						.Where(sh => sh.Shipper.ShipperId == ol.Order.Shipper.ShipperId)
						.Select(sh => sh.Shipper.PhoneNumber)
						.FirstOrDefault()
				}).ToList();

			Assert.That(list.Count, Is.EqualTo(2155));
		}

		[Test(Description = "NH-3190")]
		public void ProductsWithSubqueryReturningBoolFirstOrDefaultEq()
		{
			var result = (from p in db.Products
						  where (from c in db.Categories
								 where c.Name == "Confections"
								 && c == p.Category
								 select p.Discontinued).FirstOrDefault() == false
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}

		[Test(Description = "NH-3190")]
		public void CategoriesWithFirstProductIsNotDiscouned()
		{
			var result = (from c in db.Categories
						  where c.Products.OrderBy(p => p.ProductId).Select(p => p.Discontinued).FirstOrDefault() == false
						  select c).ToList();

			Assert.That(result.Count, Is.EqualTo(7));
		}

		[Test(Description = "NH-3190")]
		[Ignore("Not fixed yet.")]
		public void ProductsWithSubqueryReturningProjectionBoolFirstOrDefaultEq()
		{
			//NH-3190
			var result = (from p in db.Products
						  where (from c in db.Categories
								 where c.Name == "Confections"
								 && c == p.Category
								 select new{R=p.Discontinued}).FirstOrDefault().R == false
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}

		[Test(Description = "NH-3190")]
		public void ProductsWithSubqueryReturningStringFirstOrDefaultEq()
		{
			var result = (from p in db.Products
						  where (from c in db.Categories
								 where c.Name == "Confections"
								 && c == p.Category
								 select p.Name).FirstOrDefault() == p.Name
						  select p)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(13));
		}


		[Test(Description = "NH-3423")]
		public void NullComparedToNewExpressionInWhereClause()
		{
			// Construction will never be equal to null, so the ternary should be collapsed
			// to just the IfFalse expression. Without this collapsing, we cannot generate HQL.

			var result = db.Products
				.Select(p => new {Name = p.Name, Pr2 = new {ReorderLevel = p.ReorderLevel}})
				.Where(pr1 => (pr1.Pr2 == null ? (int?) null : pr1.Pr2.ReorderLevel) > 6)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(45));
		}

		private class Pr2
		{
			public int ReorderLevel { get; set; }
		}

		private class Pr1
		{
			public string Name { get; set; }
			public Pr2 Pr2 { get; set; }
		}

		[Test(Description = "NH-3423")]
		public void NullComparedToMemberInitExpressionInWhereClause()
		{
			// Construction will never be equal to null, so the ternary should be collapsed
			// to just the IfFalse expression. Without this collapsing, we cannot generate HQL.

			var result = db.Products
				.Select(p => new Pr1 { Name = p.Name, Pr2 = new Pr2 { ReorderLevel = p.ReorderLevel } })
				.Where(pr1 => (pr1.Pr2 == null ? (int?)null : pr1.Pr2.ReorderLevel) > 6)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(45));
		}
	}
}
