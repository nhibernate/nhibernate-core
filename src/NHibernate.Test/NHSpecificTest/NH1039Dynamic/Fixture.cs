using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1039Dynamic
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void Test()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var person = new Person("1")
				{
					Name = "John Doe",
					Properties =
					{
						Phones = new HashSet<object> {"555-1234", "555-4321"}
					}
				};

				s.Save(person);
				tx.Commit();
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var person = (Person) s.CreateCriteria(typeof(Person)).UniqueResult();

				Assert.That(person.ID, Is.EqualTo("1"));
				Assert.That(person.Name, Is.EqualTo("John Doe"));
				var phones = person.Properties.Phones as ISet<object>;
				Assert.That(phones, Is.Not.Null);
				Assert.That(phones.Contains("555-1234"), Is.True);
				Assert.That(phones.Contains("555-4321"), Is.True);
			}
		}
	}
}
