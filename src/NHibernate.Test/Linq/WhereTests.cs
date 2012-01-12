﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using SharpTestsEx;

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
	  [ExpectedException(typeof(InvalidOperationException))]
	  public void SingleElementWithQueryThatReturnsMultipleResults()
	  {
		 var query = (from user in db.Users
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
									 where user.Name == "ayende" && user.RegisteredAt == new DateTime(2010, 06, 17)
						 select user).FirstOrDefault();

			Assert.IsNotNull(query);
			Assert.AreEqual("ayende", query.Name);
			Assert.AreEqual(new DateTime(2010, 06, 17), query.RegisteredAt);
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
							user.Role.Entity.Output
						};

			var list = query.ToList();
			CollectionAssert.AreCountEqual(1, list);
		}

		[Test]
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
			var names = new[] { "ayende", "rahien" };
			
			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void UsersWithListContains()
		{
			var names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void UsersWithEmptyList_NH2400()
		{
			var names = new List<string>();

			var query = (from user in db.Users
						where names.Contains(user.Name)
						select user).ToList();

			Assert.That(query.Count, Is.EqualTo(0));
		}

		[Test]
		public void UsersWithEmptyEnumerable()
		{
			var allNames = new List<string> { "ayende", "rahien" };
			var names = allNames.Where(n => n == "does not exist");

			var query = (from user in db.Users
						where names.Contains(user.Name)
						select user).ToList();

			Assert.That(query.Count, Is.EqualTo(0));
		}

		[Test]
		[Ignore("inline empty list expression does not evaluate correctly")]
		public void UsersWithEmptyInlineEnumerable()
		{
			var allNames = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						where allNames.Where(n => n == "does not exist").Contains(user.Name)
						select user).ToList();

			Assert.That(query.Count, Is.EqualTo(0));
		}

		[Test]
		public void WhenTheSourceOfConstantIsICollectionThenNoThrows()
		{
			ICollection<string> names = new List<string> {"ayende", "rahien"};

			var query = (from user in db.Users
							 where names.Contains(user.Name)
							 select user);
			List<User> result = null;
			Executing.This(() => result = query.ToList()).Should().NotThrow();
			result.Count.Should().Be(2);
		}

		[Test]
		public void WhenTheSourceOfConstantIsIListThenNoThrows()
		{
			IList<string> names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
									 where names.Contains(user.Name)
									 select user);
			List<User> result = null;
			Executing.This(() => result = query.ToList()).Should().NotThrow();
			result.Count.Should().Be(2);
		}

		[Test]
		public void TimesheetsWithCollectionContains()
		{
			var entry = session.Get<TimesheetEntry>(1);

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
			var names = new[] { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void UsersWithListNotContains()
		{
			var names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void TimesheetsWithCollectionNotContains()
		{
			var entry = session.Get<TimesheetEntry>(1);

			var query = (from sheet in db.Timesheets
						 where !sheet.Entries.Contains(entry)
						 select sheet).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void TimesheetsWithEnumerableContains()
		{
			var user = session.Get<User>(1);

			var query = (from sheet in db.Timesheets
						 where sheet.Users.Contains(user)
						 select sheet).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void SearchOnObjectTypeWithExtensionMethod()
		{
			var query = (from o in session.Query<Animal>()
						 select o).OfType<Dog>().ToList();

			Assert.AreEqual(2, query.Count);
		}

				[Test(Description = "Reported as bug NH-2206")]
				public void SearchOnObjectTypeUpCastWithExtensionMethod()
				{
					var query = (from o in session.Query<Dog>()
											 select o).Cast<Animal>().ToList();

					Assert.AreEqual(2, query.Count);
				}

				[Test(Description = "Reported as bug NH-2206")]
				public void SearchOnObjectTypeCast()
				{
					var query = (from Dog o in session.Query<Dog>()
											 select o).ToList();

					Assert.AreEqual(2, query.Count);
				}

		[Test]
		public void SearchOnObjectTypeWithIsKeyword()
		{
			var query = (from o in session.Query<Animal>()
						 where o is Dog
						 select o).ToList();

			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void BitwiseQuery() 
		{
			var featureSet = FeatureSet.HasMore;
			var query = (from o in session.Query<User>()
						 where (o.Features & featureSet) == featureSet
						 select o).ToList();

			Assert.IsNotNull(query);
		}

		[Test]
		public void BitwiseQuery2()
		{
			var featureSet = FeatureSet.HasAll;
			var query = (from o in session.Query<User>()
						 where (o.Features & featureSet) == featureSet
						 select o).ToList();

			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void BitwiseQuery3()
		{
			var featureSet = FeatureSet.HasThat;
			var query = (from o in session.Query<User>()
						 where ((o.Features | featureSet) & featureSet) == featureSet
						 select o).ToList();

			Assert.AreEqual(3, query.Count);
		}

		[Test]
		public void OfTypeWithWhereAndProjection()
		{
			// NH-2375
			(from a
				in session.Query<Animal>().OfType<Cat>()
			 where a.Pregnant
			 select a.Id).FirstOrDefault();
		}

		[Test]
		public void OfTypeWithWhere()
		{
			// NH-2375
			(from a
				in session.Query<Animal>().OfType<Cat>()
			 where a.Pregnant
			 select a).FirstOrDefault();
		}

		[Test]
		public void TimeSheetsWithSamePredicateTwoTimes()
		{
			//NH-3009
			Expression<Func<Timesheet, bool>> predicate = timesheet => timesheet.Entries.Any(e => e.Id != 1);

			var query = db.Timesheets
				.Where(predicate)
				.Where(predicate)
				.ToList();

			Assert.AreEqual(2, query.Count);
		}
	}
}
