using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Dynamic.Core;

namespace NHibernate.Test.NHSpecificTest.NH2664Dynamic
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] {"NHSpecificTest.NH2664Dynamic.Mappings.hbm.xml"};

		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var properties1 = new Dictionary<string, object>
				{
					["Name"] = "First Product",
					["Description"] = "First Description"
				};
				session.Save(
					new Product
					{
						ProductId = "1",
						Properties = properties1
					});

				var properties2 = new
				{
					Name = "Second Product",
					Description = "Second Description"
				};
				session.Save(
					new Product
					{
						ProductId = "2",
						Properties = properties2
					});

				dynamic properties3 = new ExpandoObject();
				properties3.Name = "Third Product";
				properties3.Description = "Third Description";
				session.Save(
					new Product
					{
						ProductId = "3",
						Properties = properties3
					});

				tran.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.CreateQuery("delete from Product").ExecuteUpdate();
				tran.Commit();
			}
		}

		[Test]
		public void Query_DynamicComponent()
		{
			using (var session = OpenSession())
			{
				var product = session
				              .Query<Product>()
				              .Where("Properties.Name == @0", "First Product")
				              .Single();

				Assert.That(product, Is.Not.Null);
				Assert.That((object) product.Properties["Name"], Is.EqualTo("First Product"));
				Assert.That((object) product.Properties.Name, Is.EqualTo("First Product"));
			}
		}

		[Test]
		public void Multiple_Query_Does_Not_Cache()
		{
			using (var session = OpenSession())
			{
				// Query by name
				var product1 = session.Query<Product>().Where("Properties.Name == @0", "First Product").Single();
				Assert.That(product1.ProductId, Is.EqualTo("1"));

				// Query by description (this test is to verify that the dictionary
				// index isn't cached from the query above.
				var product2 = session.Query<Product>().Where("Properties.Description == @0", "Second Description").Single();
				Assert.That(product2.ProductId, Is.EqualTo("2"));
			}
		}
	}
}
