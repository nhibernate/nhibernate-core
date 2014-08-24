using System.Collections;
using NHibernate.Criterion;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3139
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
				return new string[]
					{
						"NHSpecificTest.NH3139.Mappings.hbm.xml"
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
					Brand brand = new Brand(){Name = "Brand"};
					session.Save(brand);

					//this product has no inventory row
					Product product = new Product();
					product.Name = "First";
					product.Brand = brand;
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
					session.Delete("from Inventory");
					session.Delete("from Brand");
					tran.Commit();
				}
			}

		}

		[Test]
		public void Inventory_is_null()
		{
			using (var session = OpenSession())
			{
				Product product = session.CreateCriteria(typeof (Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.IsNotNull(product);
				Assert.IsNull(product.Inventory);
				//Second check will fail because we now have a proxy
				Assert.IsTrue(product.Inventory == null);
			}
		}

		[Test]
		public void Other_entities_are_still_proxies()
		{
			using (var session = OpenSession())
			{
				Product product = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.IsNotNull(product);
				Assert.That(product.Brand is INHibernateProxy);
			}
		}
	}
}
