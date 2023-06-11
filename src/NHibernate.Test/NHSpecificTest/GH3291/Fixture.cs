using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3291
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Person { Name = "Bob", DateOfBirth = new DateTime(2009, 12, 23) };
				session.Save(e1);

				var e2 = new Person { Name = "Sally", DateOfBirth = new DateTime(2018, 9, 30) };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void Linq()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				DateTime? dateOfSearch = null;

				var result = (
					from person in session.Query<Person>()
					where dateOfSearch == null || person.DateOfBirth > dateOfSearch
					select person).ToList();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}
		
		[Test]
		public void Hql()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				DateTime? dateOfSearch = null;

				var result =
					session.CreateQuery("from Person where :DateOfSearch is null OR DateOfBirth > :DateOfSearch")
						.SetParameter("DateOfSearch", dateOfSearch, NHibernateUtil.DateTime)
						.List<Person>();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}
	}
}
