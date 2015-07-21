using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MiscellaneousTextFixture : LinqTestCase
	{
		[Category("WHERE")]
		[Test(Description = "This sample uses WHERE to filter for Shippers using a Guid property.")]
		public void WhereUsingGuidProperty()
		{
			var q =
				from s in db.Shippers
				where s.Reference == new Guid("6DFCD0D7-4D2E-4525-A502-3EA9AA52E965")
				select s;

			AssertByIds(q, new[] { 2 }, x => x.ShipperId);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
        [Test(Description = "This sample uses Count to find the number of Orders placed before yesterday in the database.")]
        public void CountWithWhereClause()
        {
            var q = from o in db.Orders where o.OrderDate <= DateTime.Today.AddDays(-1) select o;

            var count = q.Count();

            Console.WriteLine(count);
        }

        [Category("From NHUser list")]
        [Test(Description = "Telerik grid example, http://www.telerik.com/community/forums/aspnet-mvc/grid/grid-and-nhibernate-linq.aspx")]
        public void TelerikGridWhereClause()
        {
            Expression<Func<Customer, bool>> filter = c => c.ContactName.ToLower().StartsWith("a");
            IQueryable<Customer> value = db.Customers;

            var results = value.Where(filter).ToList();

            Assert.IsFalse(results.Where(c => !c.ContactName.ToLower().StartsWith("a")).Any());
        }

        [Category("From NHUser list")]
        [Test(Description = "Predicated count on a child list")]
        public void PredicatedCountOnChildList()
        {
            var results = (from c in db.Customers
                           select new
                                      {
                                          c.ContactName,
                                          Count = c.Orders.Count(o => o.Employee.EmployeeId == 4)
                                      }).ToList();

            Assert.AreEqual(91, results.Count());
            Assert.AreEqual(2, results.Where(c => c.ContactName == "Maria Anders").Single().Count);
            Assert.AreEqual(4, results.Where(c => c.ContactName == "Thomas Hardy").Single().Count);
            Assert.AreEqual(0, results.Where(c => c.ContactName == "Elizabeth Brown").Single().Count);
        }

        [Category("From NHUser list")]
        [Test(Description = "Reference an outer object in a predicate")]
        public void ReferenceToOuter()
        {
           var results = from c in db.Customers
                  where
                  c.Orders.Any(o => o.ShippedTo == c.CompanyName)
                  select c;

            Assert.AreEqual(85, results.Count());
        }

        [Category("Paging")]
		[Test(Description = "This sample uses a where clause and the Skip and Take operators to select " +
							"the second, third and fourth pages of products")]
		public void TriplePageSelection()
		{
			IQueryable<Product> q = (
			                        	from p in db.Products
			                        	where p.ProductId > 1
			                        	orderby p.ProductId
			                        	select p
			);

            IQueryable<Product> page2 = q.Skip(5).Take(5);
            IQueryable<Product> page3 = q.Skip(10).Take(5);
            IQueryable<Product> page4 = q.Skip(15).Take(5);

			var firstResultOnPage2 = page2.First();
			var firstResultOnPage3 = page3.First();
			var firstResultOnPage4 = page4.First();

			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage3.ProductId);
			Assert.AreNotEqual(firstResultOnPage3.ProductId, firstResultOnPage4.ProductId);
			Assert.AreNotEqual(firstResultOnPage2.ProductId, firstResultOnPage4.ProductId);
		}

        [Test]
        public void SelectFromObject()
        {
            using (var s = OpenSession())
            {
                var hql = s.CreateQuery("from System.Object o").List();

                var r = from o in s.Query<object>() select o;

                var l = r.ToList();

                Assert.AreEqual(hql.Count, l.Count);
            } 
        }
	}
}
