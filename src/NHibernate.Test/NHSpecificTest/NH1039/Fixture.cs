using System;
using System.Collections.Generic;
using NUnit.Framework;


namespace NHibernate.Test.NHSpecificTest.NH1039
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1039";  }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void test()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person = new Person("1");
				person.Name = "John Doe";
				var set = new HashSet<object>();
				set.Add("555-1234");
				set.Add("555-4321");
				person.Properties.Add("Phones", set);

				s.Save(person);
				tx.Commit();
			}
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person = (Person)s.CreateCriteria(typeof(Person)).UniqueResult();

				Assert.AreEqual("1", person.ID);
				Assert.AreEqual("John Doe", person.Name);
				Assert.AreEqual(1, person.Properties.Count);
				Assert.That(person.Properties["Phones"], Is.InstanceOf<ISet<object>>());
				Assert.IsTrue(((ISet<object>) person.Properties["Phones"]).Contains("555-1234"));
				Assert.IsTrue(((ISet<object>) person.Properties["Phones"]).Contains("555-4321"));
			}
		}
	}
}
