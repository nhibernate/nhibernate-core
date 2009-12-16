using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Test.Linq.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class WhereTests : LinqTestCase
	{
		[Test]
		public void NoWhereClause()
		{
			var query = (from user in db.Users
						 select user).ToList();
			Assert.AreEqual(3, query.Count);
		}

		[Test]
		public void OrWithTrueReducesTo1Eq1Clause()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" || true
						 select user).ToList();
			Assert.AreEqual(3, query.Count);
		}
		[Test]
		public void AndWithTrueReducesTo1Eq0Clause()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" && false
						 select user).ToList();
			Assert.AreEqual(0, query.Count);
		}

		[Test]
		public void WhereWithConstantExpression()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende"
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void FirstElementWithWhere()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende"
						 select user).First();
			Assert.AreEqual("ayende", query.Name);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FirstElementWithQueryThatReturnsNoResults()
		{
			var query = (from user in db.Users
						 where user.Name == "xxx"
						 select user).First();
		}

		[Test]
		public void FirstOrDefaultElementWithQueryThatReturnsNoResults()
		{
			var query = (from user in db.Users
						 where user.Name == "xxx"
						 select user).FirstOrDefault();

			Assert.IsNull(query);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SingleElementWithQueryThatReturnsNoResults()
		{
			var query = (from user in db.Users
						 where user.Name == "xxx"
						 select user).Single();
		}

		[Test]
		public void SingleOrDefaultElementWithQueryThatReturnsNoResults()
		{
			var query = (from user in db.Users
						 where user.Name == "xxx"
						 select user).SingleOrDefault();

			Assert.IsNull(query);
		}

		[Test]
		public void UsersRegisteredAtOrAfterY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt >= new DateTime(2000, 1, 1)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}


		[Test]
		public void UsersRegisteredAtOrAfterY2K_And_Before2001()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt >= new DateTime(2000, 1, 1) && user.RegisteredAt <= new DateTime(2001, 1, 1)
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersByNameAndRegistrationDate()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" && user.RegisteredAt == DateTime.Today
						 select user).FirstOrDefault();

            Assert.IsNotNull(query);
			Assert.AreEqual("ayende", query.Name);
			Assert.AreEqual(DateTime.Today, query.RegisteredAt);
		}

		[Test]
		public void UsersRegisteredAfterY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt > new DateTime(2000, 1, 1)
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void UsersRegisteredBeforeY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt < new DateTime(2000, 1, 1)
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2KAndNamedNHibernate()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1) && user.Name == "nhibernate"
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2KOrNamedNHibernate()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1) || user.Name == "nhibernate"
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void TestDataContext()
		{
			var query = from u in db.Users
						where u.Name == "ayende"
						select u;
			Assert.AreEqual(1, query.Count());
		}

		[Test]
		public void UsersWithNullLoginDate()
		{
			var query = (from user in db.Users
						 where user.LastLoginDate == null
						 select user).ToList();

			CollectionAssert.AreCountEqual(2, query);
		}

		[Test]
		public void UsersWithNonNullLoginDate()
		{
			var query = (from user in db.Users
						 where user.LastLoginDate != null
						 select user).ToList();

			CollectionAssert.AreCountEqual(1, query);
		}

		[Test]
		public void UsersWithDynamicInvokedExpression()
		{
			//simulate dynamically created where clause
			Expression<Func<User, bool>> expr1 = u => u.Name == "ayende";
			Expression<Func<User, bool>> expr2 = u => u.Name == "rahien";

			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			var dynamicWhereClause = Expression.Lambda<Func<User, bool>>
				  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);

			var query = db.Users.Where(dynamicWhereClause).ToList();

			CollectionAssert.AreCountEqual(2, query);
		}

		[Test]
		public void UsersWithComponentProperties()
		{
			var query = from user in db.Users
						where user.Component.Property1 == "test1"
						select user;

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
		public void UsersWithNestedComponentProperties()
		{
			var query = from user in db.Users
						where user.Component.OtherComponent.OtherProperty1 == "othertest1"
						select user;

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
		public void UsersWithAssociatedEntityProperties()
		{
			var query = from user in db.Users
						where user.Role.Name == "Admin" && user.Role.IsActive
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
		public void UsersWithEntityPropertiesThreeLevelsDeep()
		{
			var query = from user in db.Users
						where user.Role.Entity.Output != null
						select new
						{
							user.Name,
							RoleName = user.Role.Name,
							Output = user.Role.Entity.Output
						};

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
        [Ignore("Drill down into entity requires explicit left join")]
		public void UsersWithoutRole()
		{
			var query = from user in db.Users
						where user.Role == null
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
		public void UsersWithRole()
		{
			var query = from user in db.Users
						where user.Role != null
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			CollectionAssert.AreCountEqual(2, list);
		}

		[Test]
		public void UsersWithStringContains()
		{
			var query = (from user in db.Users
						 where user.Name.Contains("yend")
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersWithArrayContains()
		{
			var names = new string[] { "ayende", "rahien" };
            
		    var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void UsersWithListContains()
		{
			var names = new List<string>() { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void TimesheetsWithCollectionContains()
		{
			TimesheetEntry entry = session.Get<TimesheetEntry>(1);

			var timesheet = (from sheet in db.Timesheets
							 where sheet.Entries.Contains(entry)
							 select sheet).Single();

			Assert.AreEqual(2, timesheet.Id);
		}

		[Test]
		public void UsersWithStringNotContains()
		{
			var query = (from user in db.Users
						 where !user.Name.Contains("yend")
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void UsersWithArrayNotContains()
		{
			var names = new string[] { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersWithListNotContains()
		{
			var names = new List<string>() { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void TimesheetsWithCollectionNotContains()
		{
            TimesheetEntry entry = session.Get<TimesheetEntry>(1);

			var query = (from sheet in db.Timesheets
						 where !sheet.Entries.Contains(entry)
						 select sheet).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void TimesheetsWithEnumerableContains()
		{
			User user = session.Get<User>(1);

			var query = (from sheet in db.Timesheets
						 where sheet.Users.Contains(user)
						 select sheet).ToList();

			Assert.AreEqual(2, query.Count);
		}
	}
}
