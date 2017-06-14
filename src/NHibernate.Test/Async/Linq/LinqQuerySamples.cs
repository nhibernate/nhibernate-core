﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;
	[TestFixture]
	public class LinqQuerySamplesAsync : LinqTestCase
	{
		[Test]
		public async Task GroupTwoQueriesAndSumAsync()
		{
			//NH-3534
			var queryWithAggregation = from o1 in db.Orders
									   from o2 in db.Orders
									   where o1.Customer.CustomerId == o2.Customer.CustomerId && o1.OrderDate == o2.OrderDate
									   group o1 by new { o1.Customer.CustomerId, o1.OrderDate } into g
									   select new { CustomerId = g.Key.CustomerId, LastOrderDate = g.Max(x => x.OrderDate) };

			var result = await (queryWithAggregation.ToListAsync());

			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result);
		}

		[Category("SELECT/DISTINCT")]
		[Test(
			Description =
				"This sample uses SELECT and anonymous types to return a sequence of just the Customers' contact names and phone numbers."
			)]
		public async Task DLinq10Async()
		{
			var q =
				from c in db.Customers
				select new {c.ContactName, c.Address.PhoneNumber};
			var items = await (q.ToListAsync());

			Assert.AreEqual(91, items.Count);

			items.Each(x =>
						   {
							   Assert.IsNotNull(x.ContactName);
							   Assert.IsNotNull(x.PhoneNumber);
						   });
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT and anonymous types to return " +
							"a sequence of just the Employees' names and phone numbers, " +
							"with the FirstName and LastName fields combined into a single field, 'Name', " +
							"and the HomePhone field renamed to Phone in the resulting sequence.")]
		public async Task DLinq11Async()
		{
			var q =
				from e in db.Employees
				select new {Name = e.FirstName + " " + e.LastName, Phone = e.Address.PhoneNumber};
			var items = await (q.ToListAsync());
			Assert.AreEqual(9, items.Count);

			items.Each(x =>
						   {
							   Assert.IsNotNull(x.Name);
							   Assert.IsNotNull(x.Phone);
						   });
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT and a conditional statment to return a sequence of product " +
							" name and product availability.")]
		public Task DLinq13Async()
		{
			try
			{
				var q =
				from p in db.Products
				select new {p.Name, Availability = p.UnitsInStock - p.UnitsOnOrder < 0 ? "Out Of Stock" : "In Stock"};

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT and a known type to return a sequence of employees' names.")]
		public Task DLinq14Async()
		{
			try
			{
				IQueryable<Name> q =
				from e in db.Employees
				select new Name {FirstName = e.FirstName, LastName = e.LastName};

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT and WHERE to return a sequence of " +
							"just the London Customers' contact names.")]
		public Task DLinq15Async()
		{
			try
			{
				IQueryable<string> q =
				from c in db.Customers
				where c.Address.City == "London"
				select c.ContactName;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT and anonymous types to return " +
							"a shaped subset of the data about Customers.")]
		public Task DLinq16Async()
		{
			try
			{
				var q =
				from c in db.Customers
				select new
						   {
							   c.CustomerId,
							   CompanyInfo = new {c.CompanyName, c.Address.City, c.Address.Country},
							   ContactInfo = new {c.ContactName, c.ContactTitle},
							   Count = c.Orders.Count()
						   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses nested queries to return a sequence of " +
							"all orders containing their OrderId, a subsequence of the " +
							"items in the order where there is a discount, and the money " +
							"saved if shipping is not included.")]
		public Task DLinq17bAsync()
		{
			try
			{
				var q =
				from o in db.Orders
				select new
						   {
							   o.OrderId,
							   DiscountedProducts =
					from od in o.OrderLines
					where od.Discount > 0.0m
					select new {od.Quantity, od.UnitPrice},
							   FreeShippingDiscount = o.Freight
						   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses Distinct to select a sequence of the unique cities " +
							"that have Customers.")]
		public Task DLinq18Async()
		{
			try
			{
				IQueryable<string> q = (
									   from c in db.Customers
									   select c.Address.City)
				.Distinct();

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Count to find the number of Customers in the database.")]
		public async Task DLinq19Async()
		{
			int q = await (db.Customers.CountAsync());
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Count to find the number of Products in the database " +
							"that are not discontinued.")]
		public async Task DLinq20Async()
		{
			int q = await (db.Products.CountAsync(p => !p.Discontinued));
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Sum to find the total freight over all Orders.")]
		public async Task DLinq21Async()
		{
			decimal? q = await (db.Orders.Select(o => o.Freight).SumAsync());
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Sum to find the total number of units on order over all Products.")]
		public async Task DLinq22Async()
		{
			int? q = await (db.Products.SumAsync(p => p.UnitsOnOrder));
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Min to find the lowest unit price of any Product.")]
		public async Task DLinq23Async()
		{
			decimal? q = await (db.Products.Select(p => p.UnitPrice).MinAsync());
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Min to find the lowest freight of any Order.")]
		public async Task DLinq24Async()
		{
			decimal? q = await (db.Orders.MinAsync(o => o.Freight));
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Max to find the latest hire date of any Employee.")]
		public async Task DLinq26Async()
		{
			DateTime? q = await (db.Employees.Select(e => e.HireDate).MaxAsync());
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Max to find the most units in stock of any Product.")]
		public async Task DLinq27Async()
		{
			int? q = await (db.Products.MaxAsync(p => p.UnitsInStock));
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Average to find the average freight of all Orders.")]
		public async Task DLinq29Async()
		{
			decimal? q = await (db.Orders.Select(o => o.Freight).AverageAsync());
			Console.WriteLine(q);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
		[Test(Description = "This sample uses Average to find the average unit price of all Products.")]
		public async Task DLinq30Async()
		{
			decimal? q = await (db.Products.AverageAsync(p => p.UnitPrice));
			Console.WriteLine(q);
		}

		[Category("ORDER BY")]
		[Test(Description = "This sample uses orderby to sort Employees by hire date.")]
		public Task DLinq36Async()
		{
			try
			{
				IOrderedQueryable<Employee> q =
				from e in db.Employees
				orderby e.HireDate
				select e;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("ORDER BY")]
		[Test(Description = "This sample uses where and orderby to sort Orders " +
							"shipped to London by freight.")]
		public Task DLinq37Async()
		{
			try
			{
				IOrderedQueryable<Order> q =
				from o in db.Orders
				where o.ShippingAddress.City == "London"
				orderby o.Freight
				select o;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("ORDER BY")]
		[Test(Description = "This sample uses orderby to sort Products " +
							"by unit price from highest to lowest.")]
		public Task DLinq38Async()
		{
			try
			{
				IOrderedQueryable<Product> q =
				from p in db.Products
				orderby p.UnitPrice descending
				select p;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("ORDER BY")]
		[Test(Description = "This sample uses a compound orderby to sort Customers " +
							"by city and then contact name.")]
		public Task DLinq39Async()
		{
			try
			{
				IOrderedQueryable<Customer> q =
				from c in db.Customers
				orderby c.Address.City , c.ContactName
				select c;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("ORDER BY")]
		[Test(Description = "This sample uses orderby to sort Orders from EmployeeId 1 " +
							"by ship-to country, and then by freight from highest to lowest.")]
		public Task DLinq40Async()
		{
			try
			{
				IOrderedQueryable<Order> q =
				from o in db.Orders
				where o.Employee.EmployeeId == 1
				orderby o.ShippingAddress.Country , o.Freight descending
				select o;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by to partition Products by " +
							"CategoryId.")]
		public async Task DLinq42Async()
		{
			IQueryable<IGrouping<int, Product>> q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select g;

			await (ObjectDumper.WriteAsync(q, 1));

			foreach (var o in q)
			{
				Console.WriteLine("\n{0}\n", o);

				foreach (Product p in o)
				{
					await (ObjectDumper.WriteAsync(p));
				}
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Max " +
							"to find the maximum unit price for each CategoryId.")]
		public Task DLinq43Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   MaxPrice = g.Max(p => p.UnitPrice)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Min " +
							"to find the minimum unit price for each CategoryId.")]
		public Task DLinq44Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   MinPrice = g.Min(p => p.UnitPrice)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Average " +
							"to find the average UnitPrice for each CategoryId.")]
		public Task DLinq45Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   AveragePrice = g.Average(p2 => p2.UnitPrice)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Sum " +
							"to find the total UnitPrice for each CategoryId.")]
		public Task DLinq46Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   TotalPrice = g.Sum(p => p.UnitPrice)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Count " +
							"to find the number of Products in each CategoryId.")]
		public Task DLinq47Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   NumProducts = g.Count()
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Count " +
							"to find the number of Products in each CategoryId " +
							"that are discontinued.")]
		public Task DLinq48Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   NumProducts = g.Count(p => p.Discontinued)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses group by and Count " +
							"to find the number of Products in each CategoryId " +
							"that are not discontinued.")]
		public Task DLinq48bAsync()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					select new
							   {
								   g.Key,
								   NumProducts = g.Count(p => !p.Discontinued)
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses a where clause after a group by clause " +
							"to find all categories that have at least 10 products.")]
		public Task DLinq49Async()
		{
			try
			{
				var q =
				from p in db.Products
				group p by p.Category.CategoryId
				into g
					where g.Count() >= 10
					select new
							   {
								   g.Key,
								   ProductCount = g.Count()
							   };

				return ObjectDumper.WriteAsync(q, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses Group By to group products by CategoryId and SupplierId.")]
		public async Task DLinq50Async()
		{
			//var prods = db.Products.ToList();

			var categories =
				from p in db.Products
				group p by new {p.Category.CategoryId, p.Supplier.SupplierId}
				into g
					select new {g.Key, g};

			var nhOutput = await (ObjectDumper.WriteAsync(categories, 1));

			var categories2 =
				from p in db.Products.ToList()
				group p by new {p.Category.CategoryId, p.Supplier.SupplierId}
				into g
					select new {g.Key, g};

			string linq2ObjectsOutput = await (ObjectDumper.WriteAsync(categories2, 1));

			Assert.AreEqual(nhOutput, linq2ObjectsOutput);
		}

		[Category("GROUP BY/HAVING")]
		[Test(Description = "This sample uses Group By to return two sequences of products. " +
							"The first sequence contains products with unit price " +
							"greater than 10. The second sequence contains products " +
							"with unit price less than or equal to 10.")]
		public Task DLinq51Async()
		{
			try
			{
				var categories =
				from p in db.Products
				group p by new {Criterion = p.UnitPrice > 10}
				into g
					select g;

				return ObjectDumper.WriteAsync(categories, 1);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Customers that have no Orders.")]
		public async Task DLinq52Async()
		{
			IQueryable<Customer> q =
				from c in db.Customers
				where !c.Orders.Any()
//                where !c.Orders.Cast<Order>().Any()
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (Customer c in q)
				Assert.IsTrue(!c.Orders.Cast<Order>().Any());
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Customers that have Orders.")]
		public async Task DLinq52bAsync()
		{
			IQueryable<Customer> q =
				from c in db.Customers
				where c.Orders.Any()
//                where c.Orders.Cast<Order>().Any()
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (Customer c in q)
				Assert.IsTrue(c.Orders.Cast<Order>().Any());
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Categories that have " +
							"at least one Discontinued product.")]
		public async Task DLinq53Async()
		{
			IQueryable<ProductCategory> q =
				from c in db.Categories
				where c.Products.Any(p => p.Discontinued)
//                where c.Products.Cast<Product>().Any(p => p.Discontinued)
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (ProductCategory c in q)
				Assert.IsTrue(c.Products.Cast<Product>().Any(p => p.Discontinued));
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Categories that have " +
							"zero Discontinued products.")]
		public async Task DLinq53bAsync()
		{
			IQueryable<ProductCategory> q =
				from c in db.Categories
				where c.Products.Any(p => !p.Discontinued)
//                where c.Products.Cast<Product>().Any(p => !p.Discontinued)
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (ProductCategory c in q)
				Assert.IsTrue(c.Products.Cast<Product>().Any(p => !p.Discontinued));
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Categories that does not have " +
							"at least one Discontinued product.")]
		public async Task DLinq53cAsync()
		{
			IQueryable<ProductCategory> q =
				from c in db.Categories
				where !c.Products.Any(p => p.Discontinued)
//                where !c.Products.Cast<Product>().Any(p => p.Discontinued)
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (ProductCategory c in q)
				Assert.IsTrue(!c.Products.Cast<Product>().Any(p => p.Discontinued));
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses Any to return only Categories that does not have " +
							"any Discontinued products.")]
		public async Task DLinq53dAsync()
		{
			IQueryable<ProductCategory> q =
				from c in db.Categories
				where !c.Products.Any(p => !p.Discontinued)
//                where !c.Products.Cast<Product>().Any(p => !p.Discontinued)
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (ProductCategory c in q)
				Assert.IsTrue(!c.Products.Cast<Product>().Any(p => !p.Discontinued));
		}

		[Category("EXISTS/IN/ANY/ALL")]
		[Test(Description = "This sample uses All to return Customers whom all of their orders " +
							"have been shipped to their own city or whom have no orders.")]
		public async Task DLinq54Async()
		{
			IQueryable<Customer> q =
				from c in db.Customers
				where c.Orders.All(o => o.ShippingAddress.City == c.Address.City)
//                where c.Orders.Cast<Order>().All(o => o.ShippingAddress.City == c.Address.City)
				select c;

			await (ObjectDumper.WriteAsync(q));

			foreach (Customer c in q)
			{
				Customer customer = c;
				Assert.IsTrue(c.Orders.Cast<Order>().All(o => o.ShippingAddress.City == customer.Address.City));
			}
		}

		[Category("WHERE")]
		[Test(Description = "This sample uses First to select the first Shipper in the table.")]
		public async Task DLinq6Async()
		{
			Shipper shipper = await (db.Shippers.FirstAsync());
			Assert.AreEqual(1, shipper.ShipperId);
		}

		[Category("TOP/BOTTOM")]
		[Test(Description = "This sample uses Take to select the first 5 Employees hired.")]
		public Task DLinq60Async()
		{
			try
			{
				IQueryable<Employee> q = (
										 from e in db.Employees
										 orderby e.HireDate
										 select e)
				.Take(5);

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("TOP/BOTTOM")]
		[Test(Description = "This sample uses Skip to select all but the 10 most expensive Products.")]
		public Task DLinq61Async()
		{
			try
			{
				IQueryable<Product> q = (
										from p in db.Products
										orderby p.UnitPrice descending
										select p)
				.Skip(10);

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("Paging")]
		[Test(Description = "This sample uses the Skip and Take operators to do paging by " +
							"skipping the first 50 records and then returning the next 10, thereby " +
							"providing the data for page 6 of the Products table.")]
		public Task DLinq62Async()
		{
			try
			{
				IQueryable<Customer> q = (
										 from c in db.Customers
										 orderby c.ContactName
										 select c)
				.Skip(50)
				.Take(10);

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("Paging")]
		[Test(Description = "This sample uses a where clause and the Take operator to do paging by, " +
							"first filtering to get only the ProductIds above 50 (the last ProductId " +
							"from page 5), then ordering by ProductId, and finally taking the first 10 results, " +
							"thereby providing the data for page 6 of the Products table.  " +
							"Note that this method only works when ordering by a unique key.")]
		public Task DLinq63Async()
		{
			try
			{
				IQueryable<Product> q = (
										from p in db.Products
										where p.ProductId > 50
										orderby p.ProductId
										select p)
				.Take(10);

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("WHERE")]
		[Test(Description = "This sample uses First to select the single Customer with CustomerId 'BONAP'.")]
		public async Task DLinq7Async()
		{
			Customer cust = await (db.Customers.FirstAsync(c => c.CustomerId == "BONAP"));
			Assert.AreEqual("BONAP", cust.CustomerId);
		}

		[Category("WHERE")]
		[Test(Description = "This sample uses First to select an Order with freight greater than 10.00.")]
		public async Task DLinq8Async()
		{
			Order ord = await (db.Orders.FirstAsync(o => o.Freight > 10.00M));
			Assert.Greater(ord.Freight, 10.00M);
		}

		[Category("SELECT/DISTINCT")]
		[Test(Description = "This sample uses SELECT to return a sequence of just the Customers' contact names.")]
		public async Task DLinq9Async()
		{
			IQueryable<string> q =
				from c in db.Customers
				select c.ContactName;
			IList<string> items = await (q.ToListAsync());
			Assert.AreEqual(91, items.Count);
			items.Each(Assert.IsNotNull);
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers in London.")]
		public Task DLinqJoin1Async()
		{
			try
			{
				IQueryable<Order> q =
				from c in db.Customers
				from o in c.Orders
//                from o in c.Orders.Cast<Order>()
				where c.Address.City == "London"
				select o;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample shows how to construct a join where one side is nullable and the other isn't.")]
		public Task DLinqJoin10Async()
		{
			try
			{
				var q =
				from o in db.Orders
				join e in db.Employees
					on o.Employee.EmployeeId equals (int?) e.EmployeeId into emps
				from e in emps
				select new {o.OrderId, e.FirstName};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers in London.")]
		public Task DLinqJoin1aAsync()
		{
			try
			{
				var q =
				from c in db.Customers
				from o in c.Orders
//                from o in c.Orders.Cast<Order>()
				where c.Address.City == "London"
				select new {o.OrderDate, o.ShippingAddress.Region};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers in London.")]
		public Task DLinqJoin1bAsync()
		{
			try
			{
				var q =
				from c in db.Customers
				from o in c.Orders
//                from o in c.Orders.Cast<Order>()
				where c.Address.City == "London"
				select new {c.Address.City, o.OrderDate, o.ShippingAddress.Region};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers.")]
		public async Task DLinqJoin1cAsync()
		{
			IQueryable<Order> q =
				from c in db.Customers
				from o in c.Orders
//                from o in c.Orders.Cast<Order>()
				select o;

			List<Order> list = await (q.ToListAsync());

			await (ObjectDumper.WriteAsync(q));
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers.")]
		public async Task DLinqJoin1dAsync()
		{
			IQueryable<DateTime?> q =
				from c in db.Customers
//                from o in c.Orders.Cast<Order>()
				from o in c.Orders
				select o.OrderDate;

			List<DateTime?> list = await (q.ToListAsync());

			await (ObjectDumper.WriteAsync(q));
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to select all orders for customers.")]
		public async Task DLinqJoin1eAsync()
		{
			IQueryable<Customer> q =
				from c in db.Customers
				from o in c.Orders
//                from o in c.Orders.Cast<Order>()
				select c;

			List<Customer> list = await (q.ToListAsync());

			await (ObjectDumper.WriteAsync(q));
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"where clause to filter for Products whose Supplier is in the USA " +
							"that are out of stock.")]
		public Task DLinqJoin2Async()
		{
			try
			{
				IQueryable<Product> q =
				from p in db.Products
				where p.Supplier.Address.Country == "USA" && p.UnitsInStock == 0
				select p;

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"from clause to filter for employees in Seattle, " +
							"and also list their territories.")]
		public Task DLinqJoin3Async()
		{
			try
			{
				var q =
				from e in db.Employees
				from et in e.Territories
//                from et in e.Territories.Cast<Territory>()
				where e.Address.City == "Seattle"
				select new {e.FirstName, e.LastName, et.Region.Description};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample uses foreign key navigation in the " +
							"select clause to filter for pairs of employees where " +
							"one employee reports to the other and where " +
							"both employees are from the same City.")]
		public Task DLinqJoin4Async()
		{
			try
			{
				var q =
				from e1 in db.Employees
				from e2 in e1.Subordinates
//                from e2 in e1.Subordinates.Cast<Employee>()
				where e1.Address.City == e2.Address.City
				select new
						   {
							   FirstName1 = e1.FirstName,
							   LastName1 = e1.LastName,
							   FirstName2 = e2.FirstName,
							   LastName2 = e2.LastName,
							   e1.Address.City
						   };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins two tables and projects results from both tables using a group join.")]
		public Task DLinqJoin5Async()
		{
			try
			{
				var q =
				from c in db.Customers 
				join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into orders
				select new {c.ContactName, OrderCount = orders.Average(x => x.Freight)};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins two tables and projects results from both tables.")]
		public Task DLinqJoin5aAsync()
		{
			try
			{
				var q =
				from c in db.Customers
				join o in db.Orders on c.CustomerId equals o.Customer.CustomerId
				select new { c.ContactName, o.OrderId };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins two tables and projects results from both tables using a group join.")]
		public Task DLinqJoin5bAsync()
		{
			try
			{
				var q = from c in db.Customers
					join o in db.Orders on c.CustomerId equals o.Customer.CustomerId
					group new { c, o } by c.ContactName
						into g
						select new { ContactName = g.Key, OrderCount = g.Average(i => i.o.Freight) };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins two tables with a composite key and projects results from both tables.")]
		public Task DLinqJoin5cAsync()
		{
			try
			{
				var q =
				from c in db.Customers
				join o in db.Orders on new {c.CustomerId} equals new {o.Customer.CustomerId}
				select new { c.ContactName, o.OrderId };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins two tables with a composite key and projects results from both tables.")]
		public Task DLinqJoin5dAsync()
		{
			try
			{
				var q =
				from c in db.Customers
				join o in db.Orders on new {c.CustomerId, HasContractTitle = c.ContactTitle != null} equals new {o.Customer.CustomerId, HasContractTitle = o.Customer.ContactTitle != null }
				select new { c.ContactName, o.OrderId };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample explictly joins three tables and projects results from each of them.")]
		public Task DLinqJoin6Async()
		{
			try
			{
				var q =
				from c in db.Customers
				join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into ords
				join e in db.Employees on c.Address.City equals e.Address.City into emps
				select new {c.ContactName, ords = ords.Count(), emps = emps.Count()};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample projects a 'let' expression resulting from a join.")]
		public Task DLinqJoin8Async()
		{
			try
			{
				var q =
				from c in db.Customers
				join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into ords
				let z = c.Address.City + c.Address.Country
				from o in ords
				select new {c.ContactName, o.OrderId, z};

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[Category("JOIN")]
		[Test(Description = "This sample shows a group join with a composite key.")]
		public async Task DLinqJoin9Async()
		{
			var expected =
				(from o in db.Orders.ToList()
				 from p in db.Products.ToList()
				 join d in db.OrderLines.ToList()
					on new {o.OrderId, p.ProductId} equals new {d.Order.OrderId, d.Product.ProductId}
					into details
				 from d in details
				 select new {o.OrderId, p.ProductId, d.UnitPrice}).ToList();

			var actual =
				await ((from o in db.Orders
				 from p in db.Products
				 join d in db.OrderLines
					on new {o.OrderId, p.ProductId} equals new {d.Order.OrderId, d.Product.ProductId}
					into details
				 from d in details
				 select new {o.OrderId, p.ProductId, d.UnitPrice}).ToListAsync());

			Assert.AreEqual(expected.Count, actual.Count);
		}

		[Category("JOIN")]
		[Test(Description = "This sample shows a join which is then grouped")]
		public Task DLinqJoin9bAsync()
		{
			try
			{
				var q = from c in db.Customers
					 join o in db.Orders on c.CustomerId equals o.Customer.CustomerId
					 group o by c into x
					 select new { CustomerName = x.Key.ContactName, Order = x };

				return ObjectDumper.WriteAsync(q);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}