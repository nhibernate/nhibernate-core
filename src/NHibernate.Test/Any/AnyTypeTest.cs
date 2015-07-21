using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Any
{
	[TestFixture]
	public class AnyTypeTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"Any.Person.hbm.xml"}; }
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public void FlushProcessing()
		{
			//http://opensource.atlassian.com/projects/hibernate/browse/HHH-1663
			ISession session = OpenSession();
			session.BeginTransaction();
			Person person = new Person();
			Address address = new Address();
			person.Data = address;
			session.SaveOrUpdate(person);
			session.SaveOrUpdate(address);
			session.Transaction.Commit();
			session.Close();

			session = OpenSession();
			session.BeginTransaction();
			person = (Person) session.Load(typeof (Person), person.Id);
			person.Name = "makingpersondirty";
			session.Transaction.Commit();
			session.Close();

			session = OpenSession();
			session.BeginTransaction();
			session.Delete(person);
			session.Delete(address);
			session.Transaction.Commit();
			session.Close();
		}
	}
}