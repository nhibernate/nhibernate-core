using Iesi.Collections;

using NUnit.Framework;

using System;
using System.Collections;

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
				HashedSet set = new HashedSet();
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
				Assert.IsInstanceOfType(typeof(ISet), person.Properties["Phones"]);
				Assert.IsTrue((person.Properties["Phones"] as ISet).Contains("555-1234"));
				Assert.IsTrue((person.Properties["Phones"] as ISet).Contains("555-4321"));
			}
		}
	}
}
