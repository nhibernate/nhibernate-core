using System;
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

			Assert.That(query.Count, Is.EqualTo(3));
		}

		[Test]
		public void OrWithTrueReducesTo1Eq1Clause()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" || true
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(3));
		}

		[Test]
		public void AndWithTrueReducesTo1Eq0Clause()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" && false
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(0));
		}

		[Test]
		public void WhereWithConstantExpression()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende"
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void FirstElementWithWhere()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende"
						 select user).First();

			Assert.That(query.Name, Is.EqualTo("ayende"));
		}

		[Test]
		public void FirstElementWithQueryThatReturnsNoResults()
		{
			var users = from user in db.Users
						where user.Name == "xxx"
						select user;

			Assert.Throws<InvalidOperationException>(() =>
				{
					users.First();
				});
		}

		[Test]
		public void FirstOrDefaultElementWithQueryThatReturnsNoResults()
		{
			var user = (from u in db.Users
						where u.Name == "xxx"
						select u).FirstOrDefault();

			Assert.That(user, Is.Null);
		}

		[Test]
		public void SingleElementWithQueryThatReturnsNoResults()
		{
			var users = from user in db.Users
						where user.Name == "xxx"
						select user;

			Assert.Throws<InvalidOperationException>(() =>
				{
					users.Single();
				});
		}

		[Test]
		public void SingleElementWithQueryThatReturnsMultipleResults()
		{
			var users = from user in db.Users
						select user;

			Assert.Throws<InvalidOperationException>(() =>
				{
					users.Single();
				});
		}

		[Test]
		public void SingleOrDefaultElementWithQueryThatReturnsNoResults()
		{
			var query = (from user in db.Users
						 where user.Name == "xxx"
						 select user).SingleOrDefault();

			Assert.That(query, Is.Null);
		}

		[Test]
		public void UsersRegisteredAtOrAfterY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt >= new DateTime(2000, 1, 1)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}


		[Test]
		public void UsersRegisteredAtOrAfterY2K_And_Before2001()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt >= new DateTime(2000, 1, 1) && user.RegisteredAt <= new DateTime(2001, 1, 1)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersByNameAndRegistrationDate()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende" && user.RegisteredAt == new DateTime(2010, 06, 17)
						 select user).FirstOrDefault();

			Assert.That(query, Is.Not.Null);
			Assert.That(query.Name, Is.EqualTo("ayende"));
			Assert.That(query.RegisteredAt, Is.EqualTo(new DateTime(2010, 06, 17)));
		}

		[Test]
		public void UsersRegisteredAfterY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt > new DateTime(2000, 1, 1)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersRegisteredBeforeY2K()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt < new DateTime(2000, 1, 1)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2KAndNamedNHibernate()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1) && user.Name == "nhibernate"
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersRegisteredAtOrBeforeY2KOrNamedNHibernate()
		{
			var query = (from user in db.Users
						 where user.RegisteredAt <= new DateTime(2000, 1, 1) || user.Name == "nhibernate"
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TestDataContext()
		{
			var query = from u in db.Users
						where u.Name == "ayende"
						select u;

			Assert.That(query.Count(), Is.EqualTo(1));
		}

		[Test]
		public void UsersWithNullLoginDate()
		{
			var query = (from user in db.Users
						 where user.LastLoginDate == null
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithNonNullLoginDate()
		{
			var query = (from user in db.Users
						 where user.LastLoginDate != null
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
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

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithComponentProperties()
		{
			var query = from user in db.Users
						where user.Component.Property1 == "test1"
						select user;

			var list = query.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithNestedComponentProperties()
		{
			var query = from user in db.Users
						where user.Component.OtherComponent.OtherProperty1 == "othertest1"
						select user;

			var list = query.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithAssociatedEntityProperties()
		{
			var query = from user in db.Users
						where user.Role.Name == "Admin" && user.Role.IsActive
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
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
			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithoutRole()
		{
			var query = from user in db.Users
						where user.Role == null
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithRole()
		{
			var query = from user in db.Users
						where user.Role != null
						select new { user.Name, RoleName = user.Role.Name };

			var list = query.ToList();
			Assert.That(list.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithStringContains()
		{
			var query = (from user in db.Users
						 where user.Name.Contains("yend")
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test(Description = "NH-3261")]
		public void UsersWithStringContainsAndNotNullName()
		{
			// ReSharper disable SimplifyConditionalTernaryExpression
			var query = (from u in db.Users
						 where u.Name == null ? false : u.Name.Contains("yend")
						 select u).ToList();
			// ReSharper restore SimplifyConditionalTernaryExpression

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test(Description = "NH-3261")]
		public void UsersWithStringContainsAndNotNullNameHQL()
		{
			var users = session.CreateQuery("from User u where (case when u.Name is null then 'false' else (case when u.Name LIKE '%yend%' then 'true' else 'false' end) end) = 'true'").List<User>();

			Assert.That(users.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithArrayContains()
		{
			var names = new[] { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithListContains()
		{
			var names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
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
		[Ignore("Inline empty list expression does not evaluate correctly")]
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
			ICollection<string> names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user);

			List<User> result = null;
			Assert.DoesNotThrow(() =>
				{
					result = query.ToList();
				});

			Assert.That(result.Count, Is.EqualTo(2));
		}

		[Test]
		public void WhenTheSourceOfConstantIsIListThenNoThrows()
		{
			IList<string> names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where names.Contains(user.Name)
						 select user);

			List<User> result = null;
			Assert.DoesNotThrow(() =>
				{
					result = query.ToList();
				});

			Assert.That(result.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimesheetsWithCollectionContains()
		{
			var entry = session.Get<TimesheetEntry>(1);

			var timesheet = (from sheet in db.Timesheets
							 where sheet.Entries.Contains(entry)
							 select sheet).Single();

			Assert.That(timesheet.Id, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithStringNotContains()
		{
			var query = (from user in db.Users
						 where !user.Name.Contains("yend")
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void UsersWithArrayNotContains()
		{
			var names = new[] { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void UsersWithListNotContains()
		{
			var names = new List<string> { "ayende", "rahien" };

			var query = (from user in db.Users
						 where !names.Contains(user.Name)
						 select user).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void TimesheetsWithCollectionNotContains()
		{
			var entry = session.Get<TimesheetEntry>(1);

			var query = (from sheet in db.Timesheets
						 where !sheet.Entries.Contains(entry)
						 select sheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void TimesheetsWithEnumerableContains()
		{
			var user = session.Get<User>(1);

			var query = (from sheet in db.Timesheets
						 where sheet.Users.Contains(user)
						 select sheet).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void SearchOnObjectTypeWithExtensionMethod()
		{
			var query = (from o in session.Query<Animal>()
						 select o).OfType<Dog>().ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2206")]
		public void SearchOnObjectTypeUpCastWithExtensionMethod()
		{
			var query = (from o in session.Query<Dog>()
						 select o).Cast<Animal>().ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test(Description = "NH-2206")]
		public void SearchOnObjectTypeCast()
		{
			var query = (from Dog o in session.Query<Dog>()
						 select o).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void SearchOnObjectTypeWithIsKeyword()
		{
			var query = (from o in session.Query<Animal>()
						 where o is Dog
						 select o).ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void BitwiseQuery()
		{
			var featureSet = FeatureSet.HasMore;
			var query = (from o in session.Query<User>()
						 where (o.Features & featureSet) == featureSet
						 select o).ToList();

			Assert.That(query, Is.Not.Null);
		}

		[Test]
		public void BitwiseQuery2()
		{
			var featureSet = FeatureSet.HasAll;
			var query = (from o in session.Query<User>()
						 where (o.Features & featureSet) == featureSet
						 select o).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void BitwiseQuery3()
		{
			var featureSet = FeatureSet.HasThat;
			var query = (from o in session.Query<User>()
						 where ((o.Features | featureSet) & featureSet) == featureSet
						 select o).ToList();

			Assert.That(query.Count, Is.EqualTo(3));
		}

		[Test(Description = "NH-2375")]
		public void OfTypeWithWhereAndProjection()
		{
			(from a in session.Query<Animal>().OfType<Cat>()
			 where a.Pregnant
			 select a.Id).FirstOrDefault();
		}

		[Test(Description = "NH-2375")]
		public void OfTypeWithWhere()
		{
			(from a in session.Query<Animal>().OfType<Cat>()
			 where a.Pregnant
			 select a).FirstOrDefault();
		}

		[Test(Description = "NH-3009")]
		public void TimeSheetsWithSamePredicateTwoTimes()
		{
			Expression<Func<Timesheet, bool>> predicate = timesheet => timesheet.Entries.Any(e => e.Id != 1);

			var query = db.Timesheets
						  .Where(predicate)
						  .Where(predicate)
						  .ToList();

			Assert.That(query.Count, Is.EqualTo(2));
		}

		[Test]
		public void AnimalsWithFathersSerialNumberListContains()
		{
			var serialNumbers = new List<string> { "5678", "789" };
			var query = (from animal in db.Animals
						 where animal.Father != null && serialNumbers.Contains(animal.Father.SerialNumber)
						 select animal).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}

		[Test]
		public void AnimalsWithFathersSerialNumberListContainsWithLocalVariable()
		{
			var serialNumbers = new List<string> { "5678", "789" };
			var query = (from animal in db.Animals
						 let father = animal.Father
						 where father != null && serialNumbers.Contains(father.SerialNumber)
						 select animal).ToList();

			Assert.That(query.Count, Is.EqualTo(1));
		}
	}
}
