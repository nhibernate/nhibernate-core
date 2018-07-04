using System.Collections;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq.Expressions;
using System;

namespace NHibernate.Test.NHSpecificTest.NH2664
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get
			{
				return new[]
					{
						"NHSpecificTest.NH2664.Mappings.hbm.xml"
					};
			}
		}

		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					Product product = new Product();
					product.ProductId = "1";
					product.Properties["Name"] = "First Product";
					product.Properties["Description"] = "First Description";

					session.Save(product);

					product = new Product();
					product.ProductId = "2";
					product.Properties["Name"] = "Second Product";
					product.Properties["Description"] = "Second Description";

					session.Save(product);

					product = new Product();
					product.ProductId = "3";
					product.Properties["Name"] = "val";
					product.Properties["Description"] = "val";

					session.Save(product);

					tran.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					session.Delete("from Product");
					tran.Commit();
				}
			}

		}

		[Test]
		public void Query_DynamicComponent()
		{
			using (var session = OpenSession())
			{
				// Gets translated to SQL, which does not care about reference comparison vs value comparison.
#pragma warning disable CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
				var product =
					(from p in session.Query<Product>() where p.Properties["Name"] == "First Product" select p).Single();
#pragma warning restore CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'

				Assert.IsNotNull(product);
				Assert.AreEqual("First Product", product.Properties["Name"]);
			}
		}

		[Test]
		public void Multiple_Query_Does_Not_Cache()
		{
			using (var session = OpenSession())
			{
				// Gets translated to SQL, which does not care about reference comparison vs value comparison.
#pragma warning disable CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
				// Query by name
				var product1 = (from p in session.Query<Product>()
								where p.Properties["Name"] == "First Product"
								select p).Single();
				Assert.That(product1.ProductId, Is.EqualTo("1"));

				// Query by description (this test is to verify that the dictionary
				// index isn't cached from the query above.
				var product2 = (from p in session.Query<Product>()
								where p.Properties["Description"] == "Second Description"
								select p).Single();
				Assert.That(product2.ProductId, Is.EqualTo("2"));
#pragma warning restore CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
			}
		}

		[Test]
		public void Different_Key_In_DynamicComponentDictionary_Returns_Different_Keys()
		{
			using (var session = OpenSession())
			{
				// Gets translated to SQL, which does not care about reference comparison vs value comparison.
#pragma warning disable CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
				Expression<Func<IEnumerable>> key1 = () => (from a in session.Query<Product>() where a.Properties["Name"] == "val" select a);
				Expression<Func<IEnumerable>> key2 = () => (from a in session.Query<Product>() where a.Properties["Description"] == "val" select a);
#pragma warning restore CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'

				var nhKey1 = new NhLinqExpression(key1.Body, Sfi);
				var nhKey2 = new NhLinqExpression(key2.Body, Sfi);

				Assert.AreNotEqual(nhKey1.Key, nhKey2.Key);
			}
		}
	}
}