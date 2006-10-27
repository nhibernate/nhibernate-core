using System;
using System.Collections;
using NHibernate.Expression;
using NUnit.Framework;
using NHibernate.Test.NHSpecificTest.NH593;

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
            get
            {
                return new string[] { "NHSpecificTest.NH593.Mappings.hbm.xml" };
            }
        }

		protected override void OnSetUp()
        {
            // Create some objects
            using (ISession session = OpenSession())
            {
            	Blog blog = new Blog("bar");
            	post1 = new Post("p1");
            	this.post1.Blog = blog;
            	post2 = new Post("p2");
            	this.post2.Blog = blog;
				session.Save(blog);
				session.Save(this.post1);
				session.Save(this.post2);
            	
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            using (ISession s = sessions.OpenSession())
            {
                s.Delete("from Blog");
				s.Delete("from Post");
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
