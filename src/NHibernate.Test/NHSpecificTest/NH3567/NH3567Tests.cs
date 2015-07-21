using NUnit.Framework;
using NHibernate.Criterion;
using NHibernate.Dialect;

namespace NHibernate.Test.NHSpecificTest.NH3567
{
	[TestFixture]
	public class NH3567Tests : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = this.OpenSession())
			{
				session.BeginTransaction();
				var id = 0;

				var site1 = new Site { Id = ++id, Name = "Site 1" };
				var site2 = new Site { Id = ++id, Name = "Site 1" };
				session.Save(site1);
				session.Save(site2);


				var p1 = new Post { Id = ++id, Content = "Post 1", Site = site1 };
				var p2 = new Post { Id = ++id, Content = "Post 2", Site = site2 };

				session.Save(p1);
				session.Save(p2);

				session.Save(new Comment { Id = ++id, Content = "Comment 1.1", Post = p1 });
				session.Save(new Comment { Id = ++id, Content = "Comment 1.2", Post = p1 });
				session.Save(new Comment { Id = ++id, Content = "Comment 2.1", Post = p2 });
				session.Save(new Comment { Id = ++id, Content = "Comment 2.2", Post = p2 });
				session.Flush();
				session.Transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = this.OpenSession())
			{
				session.Delete("from Comment");
				session.Delete("from Post");
				session.Delete("from Site");
				session.Flush();
			}
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect as MsSql2005Dialect != null;
		}

		[Test]
		public void TestFlushModeAuto()
		{
			using (ISession session = this.OpenSession())
			{
				session.FlushMode = FlushMode.Auto;
				using (var transaction = session.BeginTransaction())
				{
					var post = session.QueryOver<Post>().Where(x => x.Content == "Post 1").SingleOrDefault();

					post.Content = "1";

					var comments = session.QueryOver<Comment>().JoinQueryOver(x => x.Post).Where(x => x.Content == "1").List();
					Assert.That(comments.Count, Is.EqualTo(2), "Query over returned something different than 2");

					post.Content = "I";
					var subquery = DetachedCriteria.For(typeof(Post))
												   .Add(Restrictions.Eq("Content", "I"))
												   .SetProjection(Projections.Id());
					var numberOfComments =
						session.CreateCriteria(typeof(Comment))
							.Add(Subqueries.PropertyIn("Post.Id", subquery))
							.List().Count;
					Assert.That(numberOfComments, Is.EqualTo(2), "Query with sub-query returned an invalid number of rows.");

					var site = session.Get<Site>(1);
					site.Name = "Site 3";

					subquery = DetachedCriteria.For(typeof(Post))
												   .SetProjection(Projections.Id())
												   .CreateCriteria("Site")
												   .Add(Restrictions.Eq("Name", "Site 3"));
					numberOfComments =
						session.CreateCriteria(typeof(Comment))
							.Add(Subqueries.PropertyIn("Post.Id", subquery))
							.List().Count;

					Assert.That(numberOfComments, Is.EqualTo(2), "Query with sub-query returned an invalid number of rows.");


					transaction.Rollback();

				}

			}
		}


	}


}
