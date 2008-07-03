using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1356
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanLoadWithGenericCompositeElement()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					Person person = new Person();
					person.Name = "Bob";
					person.Addresses = new List<Address>();
					person.Addresses.Add(new Address("123 Main St.", "Anytown", "LA", "12345"));
					person.Addresses.Add(new Address("456 Main St.", "Anytown", "LA", "12345"));

					session.Save(person);
					tx.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				Person person = session.CreateQuery("from Person").UniqueResult<Person>();

				Assert.IsNotNull(person);
				Assert.IsNotNull(person.Addresses);
				Assert.AreEqual(2, person.Addresses.Count);
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}
	}
}