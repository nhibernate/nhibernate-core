using System.Collections;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq.Expressions;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3571Generic
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] {"NHSpecificTest.NH3571Generic.Mappings.hbm.xml"};

		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var product = new Product {ProductId = "1"};
				product.Details.Properties["Name"] = "First Product";
				product.Details.Properties["Description"] = "First Description";

				session.Save(product);

				product = new Product {ProductId = "2"};
				product.Details.Properties["Name"] = "Second Product";
				product.Details.Properties["Description"] = "Second Description";

				session.Save(product);

				product = new Product {ProductId = "3"};
				product.Details.Properties["Name"] = "val";
				product.Details.Properties["Description"] = "val";

				session.Save(product);

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
		public void CanQueryDynamicComponentInComponent()
		{
			using (var session = OpenSession())
			{
				var product = (
					from p in session.Query<Product>()
					where (string) p.Details.Properties["Name"] == "First Product"
					select p
				).Single();

				Assert.That(product, Is.Not.Null);
				Assert.That(product.Details.Properties["Name"], Is.EqualTo("First Product"));
			}
		}

		[Test]
		public void MultipleQueriesShouldNotCache()
		{
			using (var session = OpenSession())
			{
				// Query by name
				var product1 = (
					from p in session.Query<Product>()
					where (string) p.Details.Properties["Name"] == "First Product"
					select p
				).Single();
				Assert.That(product1.ProductId, Is.EqualTo("1"));

				// Query by description (this test is to verify that the dictionary
				// index isn't cached from the query above.
				var product2 = (
					from p in session.Query<Product>()
					where (string) p.Details.Properties["Description"] == "Second Description"
					select p
				).Single();
				Assert.That(product2.ProductId, Is.EqualTo("2"));
			}
		}

		[Test]
		public void DifferentKeyInDynamicComponentDictionaryReturnsDifferentExpressionKeys()
		{
			using (var session = OpenSession())
			{
// ReSharper disable AccessToDisposedClosure Ok since the expressions aren't actually used after the using block.
				Expression<Func<IEnumerable>> key1 = () => (
					from a in session.Query<Product>()
					where (string) a.Details.Properties["Name"] == "val"
					select a
				);
				Expression<Func<IEnumerable>> key2 = () => (
					from a in session.Query<Product>()
					where (string) a.Details.Properties["Description"] == "val"
					select a);
// ReSharper restore AccessToDisposedClosure

				var nhKey1 = new NhLinqExpression(key1.Body, Sfi);
				var nhKey2 = new NhLinqExpression(key2.Body, Sfi);

				Assert.That(nhKey2.Key, Is.Not.EqualTo(nhKey1.Key));
			}
		}
	}
}