using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	// Mainly adapted from tests contributed by Nicola Tuveri on NH-2500 (NH-2500.patch file)
	[TestFixture]
	public class ConstantTest : LinqTestCase
	{
		[Test]
		[Ignore("Linq query not supported yet")]
		public void ConstantNonCached()
		{
			var c1 = (from c in db.Customers
			          select "customer1").First();

			var c2 = (from c in db.Customers
			          select "customer2").First();

			Assert.That(c1, Is.EqualTo("customer1"));
			Assert.That(c2, Is.EqualTo("customer2"));
		}

		[Test]
		public void ConstantNonCachedInAnonymousNewExpression()
		{
			var c1 = (from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new { c.CustomerId, c.ContactName, Constant = 1 }).First();

			var c2 = (from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new { c.CustomerId, c.ContactName, Constant = 2 }).First();

			Assert.That(c1.Constant, Is.EqualTo(1), "c1.Constant");
			Assert.That(c2.Constant, Is.EqualTo(2), "c2.Constant");
			Assert.That(c1.CustomerId, Is.EqualTo("ALFKI"), "c1.CustomerId");
			Assert.That(c2.CustomerId, Is.EqualTo("ANATR"), "c2.CustomerId");
		}

		[Test]
		public void ConstantNonCachedInNestedAnonymousNewExpressions()
		{
			var c1 = (from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 1,
				          Customer = new { c.CustomerId, Label = "customer1" }
			          }).First();

			var c2 = (from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 2,
				          Customer = new { c.CustomerId, Label = "customer2" }
			          }).First();

			Assert.That(c1.Number, Is.EqualTo(1), "c1.Number");
			Assert.That(c1.Customer.Label, Is.EqualTo("customer1"), "c1.Customer.Label");
			Assert.That(c2.Number, Is.EqualTo(2), "c1.Number");
			Assert.That(c2.Customer.Label, Is.EqualTo("customer2"), "c2.Customer.Label");
		}

		[Test]
		public void ConstantNonCachedInNewExpression()
		{
			var c1 = (from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new KeyValuePair<string, string>(c.ContactName, "one")).First();

			var c2 = (from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new KeyValuePair<string, string>(c.ContactName, "two")).First();

			Assert.That(c1.Value, Is.EqualTo("one"), "c1.Value");
			Assert.That(c2.Value, Is.EqualTo("two"), "c2.Value");
		}

		public class ShipperDto
		{
			public int Number { get; set; }
			public string CompanyName { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void ConstantNonCachedInMemberInitExpression()
		{
			var s1 = (from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 1,
				          CompanyName = s.CompanyName,
				          Name = "shipper1"
			          }).ToList();

			var s2 = (from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 2,
				          CompanyName = s.CompanyName,
				          Name = "shipper2"
			          }).ToList();

			Assert.That(s1, Has.Count.GreaterThan(0), "s1 Count");
			Assert.That(s2, Has.Count.GreaterThan(0), "s2 Count");
			Assert.That(s1, Has.All.Property("Number").EqualTo(1), "s1 Numbers");
			Assert.That(s1, Has.All.Property("Name").EqualTo("shipper1"), "s1 Names");
			Assert.That(s2, Has.All.Property("Number").EqualTo(2), "s2 Numbers");
			Assert.That(s2, Has.All.Property("Name").EqualTo("shipper2"), "s2 Names");
		}

		[Test]
		public void ConstantInNewArrayExpression()
		{
			var c1 = (from c in db.Categories
			          select new [] { c.Name, "category1" }).ToList();

			var c2 = (from c in db.Categories
			          select new [] { c.Name, "category2" }).ToList();

			Assert.That(c1, Has.Count.GreaterThan(0), "c1 Count");
			Assert.That(c2, Has.Count.GreaterThan(0), "c2 Count");
			Assert.That(c1.All(c => c[1] == "category1"), Is.True, "c1 second item");
			Assert.That(c2.All(c => c[1] == "category2"), Is.True, "c2 second item");
		}

		[Test]
		public void ConstantsInNewArrayExpression()
		{
			var p1 = (from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 1 },
				          { "product1", p.ProductId }
			          }).First();

			var p2 = (from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 2 },
				          { "product2", p.ProductId }
			          }).First();

			Assert.That(p1.ElementAt(0).Value == 1 && p1.ElementAt(1).Key == "product1", Is.True, "p1");
			Assert.That(p2.ElementAt(0).Value == 2 && p2.ElementAt(1).Key == "product2", Is.True, "p2");
		}

		public class InfoBuilder
		{
			private readonly int _value;

			public InfoBuilder(int value)
			{
				_value = value;
			}

			public int GetItemValue(Product p)
			{
				return _value;
			}
		}

		// Adapted from NH-2500 first test case by Andrey Titov (file NHTest3.zip)
		[Test]
		[Ignore("Not fixed yet")]
		public void ObjectConstants()
		{
			var builder = new InfoBuilder(1);
			var v1 = (from p in db.Products
			          select builder.GetItemValue(p)).First();
			builder = new InfoBuilder(2);
			var v2 = (from p in db.Products
			          select builder.GetItemValue(p)).First();

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}

		private int TestFunc(Product item, int closureValue)
		{
			return closureValue;
		}

		// Adapted from NH-3673
		[Test]
		[Ignore("Not fixed yet")]
		public void ConstantsInFuncCall()
		{
			var closureVariable = 1;
			var v1 = (from p in db.Products
			          select TestFunc(p, closureVariable)).First();
			closureVariable = 2;
			var v2 = (from p in db.Products
			          select TestFunc(p, closureVariable)).First();

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}
	}
}
