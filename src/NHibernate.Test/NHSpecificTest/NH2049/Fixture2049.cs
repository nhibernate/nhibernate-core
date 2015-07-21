using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2049
{
	[TestFixture]
	public class Fixture2049 : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var p = new Person {Id = 1, Name = "Name"};
				session.Save(p);
				var ic = new IndividualCustomer {Deleted = false, Person = p, Id = 1};
				session.Save(ic);

				var deletedPerson = new Person {Id = 2, Name = "Name Deleted"};
				session.Save(deletedPerson);
				var deletedCustomer = new IndividualCustomer {Deleted = true, Person = deletedPerson, Id = 2};
				session.Save(deletedCustomer);

				tx.Commit();
			}
		}


		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}


		[Test]
		[KnownBug("Known bug NH-2049.")]
		public void CanCriteriaQueryWithFilterOnJoinClassBaseClassProperty()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("DeletedCustomer").SetParameter("deleted", false);
				IList<Person> persons = session.CreateCriteria(typeof (Person)).List<Person>();

				Assert.That(persons, Has.Count.EqualTo(1));
				Assert.That(persons[0].Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer, Is.Not.Null);
				Assert.That(persons[0].IndividualCustomer.Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer.Deleted, Is.False);
			}
		}


		[Test]
		[KnownBug("Known bug NH-2049.", "NHibernate.Exceptions.GenericADOException")]
		public void CanHqlQueryWithFilterOnJoinClassBaseClassProperty()
		{
			using (ISession session = OpenSession())
			{
				session.EnableFilter("DeletedCustomer").SetParameter("deleted", false);
				var persons = session.CreateQuery("from Person as person left join person.IndividualCustomer as indCustomer")
					.List<Person>();

				Assert.That(persons, Has.Count.EqualTo(1));
				Assert.That(persons[0].Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer, Is.Not.Null);
				Assert.That(persons[0].IndividualCustomer.Id, Is.EqualTo(1));
				Assert.That(persons[0].IndividualCustomer.Deleted, Is.False);
			}
		}
	}
}
