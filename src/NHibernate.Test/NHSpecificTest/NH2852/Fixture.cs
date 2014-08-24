using System.Linq;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2852
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transction = session.BeginTransaction())
			{
				var city = new City { Name = "London" };
				session.Save(city);
				var address = new Address {City = city, Name = "Tower"};
				session.Save(address);
				var person = new Person {Address = address, Name = "Bill"};
				session.Save(person);
				var child = new Person {Parent = person};
				session.Save(child);
				var grandChild = new Person {Parent = child};
				session.Save(grandChild);

				transction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transction.Commit();
			}
		}
		
		[Test]
		public void ThenFetchCanExecute()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<Person>()
					.Where(p => p.Address.City.Name == "London")
					.Fetch(r => r.Address)
					.ThenFetch(a => a.City);

				var results = query.ToList();

				session.Close();

				Assert.True(NHibernateUtil.IsInitialized(results[0].Address));
				Assert.True(NHibernateUtil.IsInitialized(results[0].Address.City));
			}
		}

		[Test]
		public void AlsoFails()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<Person>()
					.Where(p => p.Parent.Parent.Name == "Bill")
					.Fetch(p => p.Parent)
					.ThenFetch(p => p.Parent);

				var results = query.ToList();

				session.Close();

				Assert.True(NHibernateUtil.IsInitialized(results[0].Parent));
				Assert.True(NHibernateUtil.IsInitialized(results[0].Parent.Parent));
			}
		}
	}
}