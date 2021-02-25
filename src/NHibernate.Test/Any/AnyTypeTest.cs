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

		protected override string[] Mappings
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
			var person = new Person();
			var address = new Address();
			//http://opensource.atlassian.com/projects/hibernate/browse/HHH-1663
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				person.Data = address;
				session.SaveOrUpdate(person);
				session.SaveOrUpdate(address);
				tran.Commit();
				session.Close();
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				person = (Person) session.Load(typeof(Person), person.Id);
				person.Name = "makingpersondirty";
				tran.Commit();
				session.Close();
			}

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete(person);
				session.Delete(address);
				tran.Commit();
				session.Close();
			}
		}
	}
}
