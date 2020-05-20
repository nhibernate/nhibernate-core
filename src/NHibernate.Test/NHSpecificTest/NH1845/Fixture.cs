using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH1845
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void LazyLoad_Initialize_AndEvict()
		{
			Category category = new Category("parent");
			category.AddSubcategory(new Category("child"));
			SaveCategory(category);

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Category loaded = session.Load<Category>(category.Id);
				NHibernateUtil.Initialize(loaded.Subcategories[0]);
				session.Evict(loaded);
				transaction.Commit();
				Assert.AreEqual("child", loaded.Subcategories[0].Name, "cannot access child");
			}
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				// first delete children
				session.CreateQuery("delete from Category where Parent != null").ExecuteUpdate();
				// then the rest
				session.CreateQuery("delete from Category").ExecuteUpdate();
				transaction.Commit();
			}
		}

		private void SaveCategory(Category category)
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.SaveOrUpdate(category);
				transaction.Commit();
			}
		}
	}
}
