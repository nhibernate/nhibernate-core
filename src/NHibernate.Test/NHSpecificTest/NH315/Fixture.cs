using System;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH315
{
	/// <summary>
	/// Summary description for Fixture.
	/// </summary>
	[TestFixture]
	[Ignore("Not working, see http://jira.nhibernate.org/browse/NH-315")]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "NHSpecificTest.NH315.Mappings.hbm.xml" };
			}
		}

		[Test]
		public void SaveClient()
		{
			Client client = new Client();
			Person person = new Person();

			client.Contacts = new ClientPersons();

			using( ISession s = OpenSession() )
			{
				s.Save( person );

				client.Contacts.PersonId = person.Id;
				client.Contacts.Person   = person;

				s.Save( client );

				s.Flush();
			}

			using( ISession s = OpenSession() )
			{
				s.Delete( client );
				s.Delete( person );

				s.Flush();
			}
		}
	}
}
