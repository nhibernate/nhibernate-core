using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1773
{
	//Also tests GH-2092 (Fails with MS SQL Ce & SQL Anywhere due to the query generating a duplicated column alias name, which these databases do not support.)
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CustomHQLFunctionsShouldBeRecognizedByTheParser()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList<PersonResult> result = s.CreateQuery("select new PersonResult(p, current_timestamp()) from Person p left join fetch p.Country").List<PersonResult>();

				Assert.AreEqual("My Name", result[0].Person.Name);
				Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].Person.Country));
				tx.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var c = new Country { Id = 100, Name = "US" };
				var p = new Person { Age = 35, Name = "My Name", Id = 1, Country = c };
				s.Save(c);
				s.Save(p);
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
