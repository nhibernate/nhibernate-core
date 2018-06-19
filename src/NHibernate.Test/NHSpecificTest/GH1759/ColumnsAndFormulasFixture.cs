using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1759
{
	[TestFixture]
	public class ColumnsAndFormulasFixture : BugTestCase
	{
		// In fact most of the test happens at mapping compilation.
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Uses some formulas with SLQ-Server transact SQL functions
			return dialect is MsSql2000Dialect;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var bob =
					new User
					{
						Name = "Bob",
						Org = "Eg",
						DepartureDate = DateTime.Parse("2012-07-01"),
						Hiring = new DateSpan
						{
							Date1 = DateTime.Parse("2001-01-01")
						}
					};
				session.Save(bob);

				var sally =
					new User
					{
						Name = "Sally",
						Org = "Eg",
						Hiring = new DateSpan
						{
							Date1 = DateTime.Parse("2006-01-08")
						}
					};
				session.Save(sally);

				var g1 = new Group { Name = "Salesperson", Org = "Eg" };
				g1.DateSpans.Add(new DateSpan { Date1 = DateTime.Parse("2000-12-31") });
				session.Save(g1);

				var g2 = new Group { Name = "Manager", Org = "Eg" };
				session.Save(g2);

				bob.Groups.Add(g1);
				g1.CommentsByUser.Add(bob, "He is gone, maybe he can be cleaned-up from the group.");
				sally.Groups.Add(g1);
				sally.Groups.Add(g2);

				session.Flush();

				// Needs to be done after the Flush, because this creates a circular reference.
				bob.MainGroupName = "Salesperson";
				sally.MainGroupName = "Salesperson";

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// HQL "delete from" does neither handle the many-to-many table, nor the element table.
				// So using ISession.Delete instead of ISession.CreateQuery("delete...
				session.Delete("from User");
				session.Delete("from Group");

				transaction.Commit();
			}
		}

		[Test]
		public void CheckManyToMany()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var bob = session.Query<User>().Single(u => u.Name == "Bob" && u.Org == "Eg");
				var sally = session.Query<User>().Single(u => u.Name == "Sally" && u.Org == "Eg");
				var salesperson = session.Query<Group>().Single(u => u.Name == "Salesperson" && u.Org == "Eg");
				var manager = session.Query<Group>().Single(u => u.Name == "Manager" && u.Org == "Eg");

				Assert.That(
					bob.Groups,
					Has.Count.EqualTo(1)
					   .And.One.EqualTo(salesperson));
				Assert.That(
					sally.Groups,
					Has.Count.EqualTo(2)
					   .And.One.EqualTo(salesperson)
					   .And.One.EqualTo(manager));
				Assert.That(
					salesperson.Users,
					Has.Count.EqualTo(2)
					   .And.One.EqualTo(bob)
					   .And.One.EqualTo(sally));
				Assert.That(
					manager.Users,
					Has.Count.EqualTo(1)
					   .And.One.EqualTo(sally));

				tx.Commit();
			}
		}

		[Test]
		public void CheckProperty()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var bob = session.Query<User>().Single(u => u.Name == "Bob" && u.Org == "Eg");
				var sally = session.Query<User>().Single(u => u.Name == "Sally" && u.Org == "Eg");

				Assert.That(
					bob.Hiring,
					Has.Property(nameof(DateSpan.Date1)).EqualTo(DateTime.Parse("2001-01-01"))
					   .And.Property(nameof(DateSpan.Date2)).EqualTo(bob.DepartureDate));
				Assert.That(
					sally.Hiring,
					Has.Property(nameof(DateSpan.Date1)).EqualTo(DateTime.Parse("2006-01-08"))
					   .And.Property(nameof(DateSpan.Date2)).EqualTo(sally.DepartureDate));

				sally.DepartureDate = DateTime.Today;

				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var sally = session.Query<User>().Single(u => u.Name == "Sally" && u.Org == "Eg");
				Assert.That(sally.DepartureDate, Is.Not.Null);
				Assert.That(
					sally.Hiring,
					Has.Property(nameof(DateSpan.Date1)).EqualTo(DateTime.Parse("2006-01-08"))
					   .And.Property(nameof(DateSpan.Date2)).EqualTo(sally.DepartureDate));

				tx.Commit();
			}
		}

		[Test]
		public void CheckElement()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var salesperson = session.Query<Group>().Single(u => u.Name == "Salesperson" && u.Org == "Eg");

				Assert.That(
					salesperson.DateSpans,
					Has.Count.GreaterThan(0)
					   .And.All.Property(nameof(DateSpan.Date2)).EqualTo(DateTime.Parse("2000-12-31").AddYears(2)));

				tx.Commit();
			}
		}

		[Test, Ignore("Needs an unrelated additional simple fix in OneToManyPersister")]
		public void CheckMapKey()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var salesperson = session.Query<Group>().Single(u => u.Name == "Salesperson" && u.Org == "Eg");

				Assert.That(
					salesperson.UsersByHiring,
					Has.Count.EqualTo(2)
					   .And.One.Property(nameof(KeyValuePair<DateSpan, User>.Key)).Property(nameof(DateSpan.Date2)).Null
					   .And.One.Property(nameof(KeyValuePair<DateSpan, User>.Key)).Property(nameof(DateSpan.Date2)).EqualTo(DateTime.Parse("2012-07-01")));

				tx.Commit();
			}
		}

		[Test]
		public void CheckMapKeyManyToMany()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var bob = session.Query<User>().Single(u => u.Name == "Bob" && u.Org == "Eg");
				var salesperson = session.Query<Group>().Single(u => u.Name == "Salesperson" && u.Org == "Eg");

				Assert.That(
					salesperson.CommentsByUser,
					Has.Count.EqualTo(1)
					   .And.One.Property(nameof(KeyValuePair<User, User>.Key)).EqualTo(bob).And.Property(nameof(KeyValuePair<User, User>.Value)).Length.GreaterThan(0));

				tx.Commit();
			}
		}
	}
}
