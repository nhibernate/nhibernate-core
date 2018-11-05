using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1760
{
	[TestFixture]
	public class MapKeyFormulasFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var bob =
					new User
					{
						Name = "Bob"
					};
				session.Save(bob);

				var sally =
					new User
					{
						Name = "Sally"
					};
				session.Save(sally);

				var g1 = new Group { Name = "Salesperson" };
				session.Save(g1);

				g1.UsersByName.Add(bob.Name, bob);
				g1.UsersByName.Add(sally.Name, sally);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// HQL "delete from" does not handle foreign key order.
				session.Delete("from User");
				session.CreateQuery("delete System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CheckMapKey()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var salesperson = session.Query<Group>().Single(u => u.Name == "Salesperson");

				Assert.That(
					salesperson.UsersByName,
					Has.Count.EqualTo(2)
					   .And.One.Property(nameof(KeyValuePair<string, User>.Key)).EqualTo("Bob")
					   .And.One.Property(nameof(KeyValuePair<string, User>.Key)).EqualTo("Sally"));

				tx.Commit();
			}
		}
	}
}
