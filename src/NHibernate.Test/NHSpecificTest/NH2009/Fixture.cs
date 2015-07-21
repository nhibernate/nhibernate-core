using System;
using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2009
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void PropertyRefToJoinedTable()
		{
			BlogPost savedBlogPost = new BlogPost();

			using (ISession session = OpenSession())
			{
				User user1 = new User();
				user1.FullName = "First User";
				user1.UserName = "User1";
				session.Save(user1);

				User user2 = new User();
				user2.FullName = "Second User";
				user2.UserName = "User2";
				session.Save(user2);

				savedBlogPost.Title = "Post 1";
				savedBlogPost.Poster = user1;
				session.Save(savedBlogPost);

				session.Flush();
				session.Clear();
			}
			
			using (ISession session = OpenSession())
			{
				var user = session.Get<BlogPost>(savedBlogPost.ID);
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from User");
				session.Delete("from BlogPost");
				tx.Commit();
			}
		}
	}
}
