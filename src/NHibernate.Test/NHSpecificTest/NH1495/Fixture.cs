using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1495
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CreateTest()
		{
			object id;

			using (ISession session = OpenSession())
			{
				var person = new Person {Name = "Nelo"};

				using (ITransaction trans = session.BeginTransaction())
				{
					session.Save(person);
					trans.Commit();
				}

				id = person.Id;
			}

			using (ISession session = OpenSession())
			{
				var person = (IPerson)session.Load(typeof(Person), id); //to work with the proxy

				Assert.IsNotNull(person);
				Assert.AreEqual("Nelo", person.Name);

				using (ITransaction trans = session.BeginTransaction())
				{
						session.Delete(person);
						trans.Commit();
				}
			}
		}

	}
}