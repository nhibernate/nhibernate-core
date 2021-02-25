using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2688
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<BasketItemMap>();
			mapper.AddMapping<ProductMap>();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Product { Name = "Printer Paper", Price = 10 };
				session.Save(e1);

				var e2 = new BasketItem { Name = "Printer Paper", Price = 10, Product = e1, OrderBy = 1 };
				session.Save(e2);

				var e3 = new BasketItem { Name = "Cable", Price = 10, Product = null, OrderBy = 2 };
				session.Save(e3);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void YourTestName()
		{
			int productId = 0;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var product = session.QueryOver<Product>()
					.Where(p => p.Name == "Printer Paper")
					.SingleOrDefault();
				productId = product.Id;
				session.Delete(product);
				transaction.Commit();
			}

			Assert.DoesNotThrow(() => { DeleteBasketItems(productId); });
		}

		private void DeleteBasketItems(int productId)
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from BasketItem where Product.Id = " + productId)
					.ExecuteUpdate();
				transaction.Commit();
			}
		}
	}
}
