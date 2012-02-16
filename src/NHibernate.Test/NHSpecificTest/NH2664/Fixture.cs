using System.Collections;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2664
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
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
				var product =
					(from p in session.Query<Product>() where p.Properties["Name"] == "First Product" select p).Single();

				Assert.IsNotNull(product);
				Assert.AreEqual("First Product", product.Properties["Name"]);
			}
		}

		[Test]
		public void Multiple_Query_Does_Not_Cache()
		{
			using (var session = OpenSession())
			{
				using (var spy = new SqlLogSpy())
				{
					//Query by name
					var product =
						(from p in session.Query<Product>() where p.Properties["Name"] == "val" select p).Single();

					Assert.That(spy.GetWholeLog(), Is.StringContaining("Name=@p0"));
				}

				using (var spy = new SqlLogSpy())
				{
					//Query by description
					var product =
						(from p in session.Query<Product>() where p.Properties["Description"] == "val" select p).Single();

					Assert.That(spy.GetWholeLog(), Is.StringContaining("Description=@p0"));
				}
			}
		}
	}
}