using System.Linq;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2846
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					// Add a test category
					var category = new Category { Id = 1, Title = "Cat 1" };
					session.Save(category);

					// Add a test post
					var post = new Post { Id = 1, Title = "Post 1", Category = category };
					session.Save(post);

					var comment1 = new Comment { Id = 1, Title = "Comment 1", Post = post };
					var comment2 = new Comment { Id = 2, Title = "Comment 2", Post = post };
					session.Save(comment1);
					session.Save(comment2);

					session.Save(post);

					// Flush the changes
					session.Flush();

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
					session.Delete("from Comment");
					session.Delete("from Post");
					session.Delete("from Category");
					tran.Commit();
				}
			}
		}


		[Test]
		public void FetchOnCountWorks()
		{
			using (var session = OpenSession())
			{

				var count = session.Query<Post>()
					.Fetch(p => p.Category)
					.FetchMany(p => p.Comments)
					.Count();

				Assert.AreEqual(1, count);

			}
		}

	}
}
