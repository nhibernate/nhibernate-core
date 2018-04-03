using System;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	/// <summary>
	/// Summary description for Fixture.
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH315"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is AbstractHanaDialect); // HANA does not support inserting a row without specifying any column values
		}

		[Test]
		public void SaveClient()
		{
			Client client = new Client();
			Person person = new Person();

			client.Contacts = new ClientPersons();

			using (ISession s = OpenSession())
			{
				s.Save(person);

				client.Contacts.PersonId = person.Id;
				client.Contacts.Person = person;

				s.Save(client);

				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Delete(client);
				s.Delete(person);

				s.Flush();
			}
		}
	}
}
