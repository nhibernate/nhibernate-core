﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NHibernate;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2760
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// All four tests translate to scalar sub-select.
			return Dialect.SupportsScalarSubSelects;
		}

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
		public async Task ShouldBeAbleToSelectUserGroupAndOrderByUserCountAsync()
		{
			if (!TestDialect.SupportsAggregatingScalarSubSelectsInOrderBy)
				Assert.Ignore("Dialect does not support aggregating scalar sub-selects in order by");

			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query =
					from ug in session.Query<UserGroup>()
					orderby ug.Users.Count()
					select ug;

				var queryResults = await (query.ToListAsync());

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(2, queryResults[0].Id);
				Assert.AreEqual(1, queryResults[1].Id);

				await (transaction.CommitAsync());
			}
		}

		[Test]
		public async Task ShouldBeAbleToSelectUserGroupWhereUserCountAsync()
		{
			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query =
					from ug in session.Query<UserGroup>()
					where ug.Users.Count() > 1
					select ug;

				var queryResults = await (query.ToListAsync());

				Assert.AreEqual(1, queryResults.Count);
				Assert.AreEqual(1, queryResults[0].Id);
				Assert.AreEqual(2, queryResults[0].Users.Count());

				await (transaction.CommitAsync());
			}
		}

		[Test]
		public async Task ShouldBeAbleToSelectUserGroupAndSelectUserIdUserCountAsync()
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

				var queryResults = await (query.ToListAsync());

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(1, queryResults[0].id);
				Assert.AreEqual(2, queryResults[0].count);
				Assert.AreEqual(2, queryResults[1].id);
				Assert.AreEqual(1, queryResults[1].count);

				await (transaction.CommitAsync());
			}
		}

		[Test]
		public async Task ShouldBeAbleToSelectUserGroupAndOrderByUserCountWithHqlAsync()
		{
			if (!TestDialect.SupportsAggregatingScalarSubSelectsInOrderBy)
				Assert.Ignore("Dialect does not support aggregating scalar sub-selects in order by");

			using (ISession session = base.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query = session.CreateQuery("select ug from UserGroup ug order by size(ug.Users)");

				var queryResults = await (query.ListAsync<UserGroup>());

				Assert.AreEqual(2, queryResults.Count);
				Assert.AreEqual(2, queryResults[0].Id);
				Assert.AreEqual(1, queryResults[1].Id);

				await (transaction.CommitAsync());
			}
		}
	}
}
