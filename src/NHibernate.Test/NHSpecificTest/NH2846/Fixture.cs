using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2846
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
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
                    session.Save(new Post { Id = 1, Title = "Post 1", Category = category });

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
                    .Count();

                Assert.AreEqual(1, count);

            }
        }
    }
}
