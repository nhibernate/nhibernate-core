using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest.SubQueries
{
	[TestFixture]
	public class SubQueriesSqlFixture : TestCase
	{
		private Post post2;
		private Post post1;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"ExpressionTest.SubQueries.Mappings.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			// Create some objects
			using (ISession session = OpenSession())
			{
				Category category = new Category("NHibernate");
				User author = new User("Josh");
				User commenter = new User("Ayende");

				Blog blog = new Blog("bar");
				blog.Users.Add(author);
				author.Blogs.Add(blog);
				post1 = new Post("p1");
				this.post1.Blog = blog;
				this.post1.Categories.Add(category);
				post2 = new Post("p2");
				this.post2.Blog = blog;

				Comment comment = new Comment("foo");
				comment.Commenter = commenter;
				comment.Post = post1;
				comment.IndexInPost = 0;
				post1.Comments.Add(comment);


				session.Save(category);
				session.Save(author);
				session.Save(commenter);
				session.Save(blog);
				session.Save(this.post1);
				session.Save(this.post2);
				session.Save(comment);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = sessions.OpenSession())
			{
				s.Delete("from Comment");
				s.Delete("from Post");
				s.Delete("from Blog");
				s.Delete("from User");
				s.Delete("from Category");
				s.Flush();
			}
		}

		[Test]
		public void CanQueryBlogByItsPosts()
		{
			DetachedCriteria dc = DetachedCriteria.For(typeof(Post), "posts")
				.SetProjection(Property.ForName("id"))
				.Add(Expression.Eq("id", post1.PostId))
				.Add(Property.ForName("posts.Blog.id").EqProperty("blog.id"));

			using (ISession s = sessions.OpenSession())
			{
				IList list = s.CreateCriteria(typeof(Blog), "blog")
					.Add(Subqueries.Exists(dc))
					.List();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void ComplexSubQuery_QueryingByGrandChildren()
		{
			DetachedCriteria comment = DetachedCriteria.For(typeof(Comment), "comment")
				.SetProjection(Property.ForName("id"))
				.Add(Property.ForName("Post.id").EqProperty("post.id"))
				.Add(Expression.Eq("Text", "foo"));

			using (ISession s = OpenSession())
			{
				DetachedCriteria dc = DetachedCriteria.For(typeof(Blog))
					.CreateCriteria("Posts", "post")
					.Add(Subqueries.Exists(comment));
				IList list = dc.GetExecutableCriteria(s).List();
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}