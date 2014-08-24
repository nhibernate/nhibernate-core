using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1773
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CustomHQLFunctionsShouldBeRecognizedByTheParser()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Country c = new Country() {Id = 100, Name = "US"};
					Person p = new Person() {Age = 35, Name = "My Name", Id=1, Country = c};
					s.Save(c);
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList<PersonResult> result = s.CreateQuery("select new PersonResult(p, current_timestamp()) from Person p left join fetch p.Country").List<PersonResult>();

				Assert.AreEqual("My Name", result[0].Person.Name);
				Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].Person.Country));
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				s.CreateQuery("delete from Country").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}
