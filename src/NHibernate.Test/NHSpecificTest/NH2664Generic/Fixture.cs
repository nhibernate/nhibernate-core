using System.Collections;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq.Expressions;
using System;

namespace NHibernate.Test.NHSpecificTest.NH2664Generic
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] {"NHSpecificTest.NH2664Generic.Mappings.hbm.xml"};

		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Save(
					new Product
					{
						ProductId = "1",
						Properties =
						{
							["Name"] = "First Product",
							["Description"] = "First Description"
						}
					});

				session.Save(
					new Product
					{
						ProductId = "2",
						Properties =
						{
							["Name"] = "Second Product",
							["Description"] = "Second Description"
						}
					});

				session.Save(
					new Product
					{
						ProductId = "3",
						Properties =
						{
							["Name"] = "val",
							["Description"] = "val"
						}
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
				var product = (
					from p in session.Query<Product>()
					where (string) p.Properties["Name"] == "First Product"
					select p
				).Single();

				Assert.That(product, Is.Not.Null);
				Assert.That(product.Properties["Name"], Is.EqualTo("First Product"));
			}
		}

		[Test]
		public void Multiple_Query_Does_Not_Cache()
		{
			using (var session = OpenSession())
			{
				// Query by name
				var product1 = (
					from p in session.Query<Product>()
					where (string) p.Properties["Name"] == "First Product"
					select p
				).Single();
				Assert.That(product1.ProductId, Is.EqualTo("1"));

				// Query by description (this test is to verify that the dictionary
				// index isn't cached from the query above.
				var product2 = (
					from p in session.Query<Product>()
					where (string) p.Properties["Description"] == "Second Description"
					select p
				).Single();
				Assert.That(product2.ProductId, Is.EqualTo("2"));
			}
		}

		[Test]
		public void Different_Key_In_DynamicComponentDictionary_Returns_Different_Keys()
		{
			using (var session = OpenSession())
			{
				Expression<Func<IEnumerable>> key1 = () =>
					from a in session.Query<Product>() 
					where (string) a.Properties["Name"] == "val" 
					select a;
				Expression<Func<IEnumerable>> key2 = () =>
					from a in session.Query<Product>()
					where (string) a.Properties["Description"] == "val"
					select a;

				var nhKey1 = new NhLinqExpression(key1.Body, Sfi);
				var nhKey2 = new NhLinqExpression(key2.Body, Sfi);

				Assert.That(nhKey2.Key, Is.Not.EqualTo(nhKey1.Key));
			}
		}
	}
}