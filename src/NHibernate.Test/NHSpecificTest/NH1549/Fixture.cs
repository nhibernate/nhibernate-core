using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1549
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		/// <summary>
		/// Verifies that an entity with a base class containing the id property 
		/// can have the id accessed without loading the entity
		/// </summary>
		[Test]
		public void CanLoadForEntitiesWithInheritedIds()
		{
			//create some related products
			var category = new CategoryWithInheritedId {Name = "Fruit"};
			var product = new ProductWithInheritedId {CategoryWithInheritedId = category};

			using (ISession session = OpenSession())
			{
				using (ITransaction trans = session.BeginTransaction())
				{
					session.Save(category);
					session.Save(product);
					trans.Commit();
				}
			}

			ProductWithInheritedId restoredProductWithInheritedId;

			//restore the product from the db in another session so that 
			//the association is a proxy
			using (ISession session = OpenSession())
			{
				restoredProductWithInheritedId = session.Get<ProductWithInheritedId>(product.Id);
			}
			
			//verify that the category is a proxy
			Assert.IsFalse(NHibernateUtil.IsInitialized(restoredProductWithInheritedId.CategoryWithInheritedId));

			//we should be able to access the id of the category outside of the session
			Assert.AreEqual(category.Id, restoredProductWithInheritedId.CategoryWithInheritedId.Id);
		}

		[Test]
		public void CanLoadForEntitiesWithTheirOwnIds()
		{
			//create some related products
			var category = new CategoryWithId { Name = "Fruit" };
			var product = new ProductWithId { CategoryWithId = category };

			using (ISession session = OpenSession())
			{
				using (ITransaction trans = session.BeginTransaction())
				{
					session.Save(category);
					session.Save(product);
					trans.Commit();
				}
			}

			ProductWithId restoredProductWithInheritedId;

			//restore the product from the db in another session so that 
			//the association is a proxy
			using (ISession session = OpenSession())
			{
				restoredProductWithInheritedId = session.Get<ProductWithId>(product.Id);
			}

			//verify that the category is a proxy
			Assert.IsFalse(NHibernateUtil.IsInitialized(restoredProductWithInheritedId.CategoryWithId));

			//we should be able to access the id of the category outside of the session
			Assert.AreEqual(category.Id, restoredProductWithInheritedId.CategoryWithId.Id);
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession()) {

				using (ITransaction trans = session.BeginTransaction()) 
				{
					session.Delete("from ProductWithId");
					session.Delete("from CategoryWithId");
					session.Delete("from ProductWithInheritedId");
					session.Delete("from CategoryWithInheritedId");
					trans.Commit();
				}
			}	
		}
	}
}