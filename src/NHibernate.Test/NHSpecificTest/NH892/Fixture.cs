using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH892
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		ISession session;

		public override string BugNumber
		{
			get { return "NH892"; }
		}

		protected override void OnSetUp()
		{
			session = OpenSession();
		}

		protected override void OnTearDown()
		{
			session.Close();
		}

		[Test]
		[Ignore("Not fixed yet. Test currently fails")]
		public void SelectWithWhereClause()
		{
			User user1 = new User();
			user1.UserName = "User1";
			session.Save(user1);

			User user2 = new User();
			user2.UserName = "User2";
			session.Save(user2);

			BlogPost post = new BlogPost();
			post.Title = "Post 1";
			post.Poster = user1;
			session.Save(post);

			session.Flush();
			session.Clear();

			User poster = (User)session.Get(typeof(User), user1.ID);

			string hql = "from BlogPost b where b.Poster = :poster";
			IList list = session.CreateQuery(hql)
				.SetParameter("poster", poster)
				.List();
			Assert.AreEqual(1, list.Count);
			BlogPost retrievedPost = (BlogPost)list[0];
			Assert.AreEqual(post.ID, retrievedPost.ID);
			Assert.AreEqual(user1.ID, retrievedPost.Poster.ID);
		}
	}
}
