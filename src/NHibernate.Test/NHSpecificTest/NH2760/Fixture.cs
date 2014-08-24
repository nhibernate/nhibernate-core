using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NHibernate;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2760
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var group1 = new UserGroup() { Id = 1, Name = "User Group 1" };
				var group2 = new UserGroup() { Id = 2, Name = "User Group 2" };

				var user1 = new User() { Id = 1, Name = "User 1" };
				var user2 = new User() { Id = 2, Name = "User 2" };

				user1.UserGroups.Add(group1);
				user1.UserGroups.Add(group2);
				user2.UserGroups.Add(group1);

				session.Save(group1);
				session.Save(group2);
				session.Save(user1);
				session.Save(user2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void ShouldBeAbleToSelectUserGroupAndOrderByUserCount()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query =
					from ug in session.Query<UserGroup>()
					orderby ug.Users.Count()
					select ug;

				var queryResults = query.ToList();

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(2, queryResults[0].Id);
				Assert.AreEqual(1, queryResults[1].Id);

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToSelectUserGroupWhereUserCount()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query =
					from ug in session.Query<UserGroup>()
					where ug.Users.Count() > 1
					select ug;

				var queryResults = query.ToList();

				Assert.AreEqual(1, queryResults.Count);
				Assert.AreEqual(1, queryResults[0].Id);
				Assert.AreEqual(2, queryResults[0].Users.Count());

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToSelectUserGroupAndSelectUserIdUserCount()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query =
					from ug in session.Query<UserGroup>()
					select new
					{
						id = ug.Id,
						count = ug.Users.Count(),
					};

				var queryResults = query.ToList();

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(1, queryResults[0].id);
				Assert.AreEqual(2, queryResults[0].count);
				Assert.AreEqual(2, queryResults[1].id);
				Assert.AreEqual(1, queryResults[1].count);

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToSelectUserGroupAndOrderByUserCountWithHql()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query = session.CreateQuery("select ug from UserGroup ug order by size(ug.Users)");

				var queryResults = query.List<UserGroup>();

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(2, queryResults[0].Id);
				Assert.AreEqual(1, queryResults[1].Id);

				transaction.Commit();
			}
		}
	}
}