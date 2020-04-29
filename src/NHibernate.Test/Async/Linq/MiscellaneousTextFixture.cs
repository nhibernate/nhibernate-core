﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;
	[TestFixture]
	public class MiscellaneousTextFixtureAsync : LinqTestCase
	{

		[Category("COUNT/SUM/MIN/MAX/AVG")]
        [Test(Description = "This sample uses Count to find the number of Orders placed before yesterday in the database.")]
        public async Task CountWithWhereClauseAsync()
        {
            var yesterday = DateTime.Today.AddDays(-1);
            var q = from o in db.Orders where o.OrderDate <= yesterday select o;

            var count = await (q.CountAsync());

            Console.WriteLine(count);
        }

        [Category("From NHUser list")]
        [Test(Description = "Telerik grid example, http://www.telerik.com/community/forums/aspnet-mvc/grid/grid-and-nhibernate-linq.aspx")]
        public async Task TelerikGridWhereClauseAsync()
        {
            Expression<Func<Customer, bool>> filter = c => c.ContactName.ToLower().StartsWith("a");
            IQueryable<Customer> value = db.Customers;

            var results = await (value.Where(filter).ToListAsync());

            Assert.IsFalse(results.Where(c => !c.ContactName.ToLower().StartsWith("a")).Any());
        }

        [Category("From NHUser list")]
        [Test(Description = "Predicated count on a child list")]
        public async Task PredicatedCountOnChildListAsync()
        {
            if (!Dialect.SupportsScalarSubSelects)
                Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");

            var results = await ((from c in db.Customers
                           select new
                                      {
                                          c.ContactName,
                                          Count = c.Orders.Count(o => o.Employee.EmployeeId == 4)
                                      }).ToListAsync());

            Assert.AreEqual(91, results.Count());
            Assert.AreEqual(2, results.Where(c => c.ContactName == "Maria Anders").Single().Count);
            Assert.AreEqual(4, results.Where(c => c.ContactName == "Thomas Hardy").Single().Count);
            Assert.AreEqual(0, results.Where(c => c.ContactName == "Elizabeth Brown").Single().Count);
        }

        [Category("From NHUser list")]
        [Test(Description = "Reference an outer object in a predicate")]
        public async Task ReferenceToOuterAsync()
        {
           var results = from c in db.Customers
                  where
                  c.Orders.Any(o => o.ShippedTo == c.CompanyName)
                  select c;

			var count = await (results.CountAsync());

			Assert.That(count,
				// Accent sensitive case
				Is.EqualTo(85).
				// Accent insensitive case (MySql has most of its case insensitive collations accent insensitive too)
				// https://bugs.mysql.com/bug.php?id=19567
				Or.EqualTo(87));
        }

        [Category("Paging")]
		[Test(Description = "This sample uses a where clause and the Skip and Take operators to select " +
							"the second, third and fourth pages of products")]
		public async Task TriplePageSelectionAsync()
		{
			var q =
				from p in db.Products
				where p.ProductId > 1
				orderby p.ProductId
				select p;

			// ToList required otherwise the First call alters the paging and test something else than paging three pages,
			// contrary o the test above description.
			var page2 = await (q.Skip(5).Take(5).ToListAsync());
			var page3 = await (q.Skip(10).Take(5).ToListAsync());
			var page4 = await (q.Skip(15).Take(5).ToListAsync());

			var firstResultOnPage2 = page2.First();
			var firstResultOnPage3 = page3.First();
			var firstResultOnPage4 = page4.First();

			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage3.ProductId);
			Assert.AreNotEqual(firstResultOnPage3.ProductId, firstResultOnPage4.ProductId);
			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage4.ProductId);
		}

		[Category("Paging")]
		[Test(Description = "NH-4061 - This sample uses a where clause and the Skip and Take operators to select " +
			"the second, third and fourth pages of products, but then add a First causing the Take to be pointless.")]
		public async Task TriplePageSelectionWithFirstAsync()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore("Not fixed: NH-4061 - Pointless Take calls screw Oracle dialect paging.");

			var q =
				from p in db.Products
				where p.ProductId > 1
				orderby p.ProductId
				select p;

			var firstResultOnPage2 = await (q.Skip(5).Take(5).FirstAsync());
			var firstResultOnPage3 = await (q.Skip(10).Take(5).FirstAsync());
			var firstResultOnPage4 = await (q.Skip(15).Take(5).FirstAsync());

			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage3.ProductId);
			Assert.AreNotEqual(firstResultOnPage3.ProductId, firstResultOnPage4.ProductId);
			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage4.ProductId);
		}

		[Test]
        public async Task SelectFromObjectAsync()
        {
            using (var s = OpenSession())
            {
                var hql = await (s.CreateQuery("from System.Object o").ListAsync());

                var r = from o in s.Query<object>() select o;

                var l = await (r.ToListAsync());

                Assert.AreEqual(hql.Count, l.Count);
            } 
        }
	}
}
