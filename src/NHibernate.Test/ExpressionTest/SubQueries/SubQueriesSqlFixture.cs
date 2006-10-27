using System;
using System.Collections;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Test.ExpressionTest.SubQueries;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest.SubQueries
{
	[TestFixture]
	public class SubQueriesSqlFixture : NHibernate.Test.TestCase
    {
		private Post post2;
		private Post post1;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		protected override IList Mappings
        {
            get
            {
				return new string[] { "ExpressionTest.SubQueries.Mappings.hbm.xml" };
            }
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
				.SetProjection(NHibernate.Expression.Property.ForName("id"))
				.Add(Expression.Expression.Eq("id", post1.PostId))
				.Add(Expression.Property.ForName("posts.Blog.id").EqProperty("blog.id"));

			using (ISession s = sessions.OpenSession())
			{
				IList list = s.CreateCriteria(typeof (Blog), "blog")
					.Add(Subqueries.Exists(dc))
					.List();
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}
