 using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH2037
{
 	[TestFixture]
 	public class Fixture : BugTestCase
	{
 		[Test]
 		public void Test()
 		{
			var country = new Country {Name = "Argentina"};

			var city = new City
			           	{
			           		CityCode = 5,
			           		Country = country,
			           		Name = "Cordoba"
			           	};

		
			using (ISession session = OpenSession())
			using(var tx = session.BeginTransaction())
			{
				session.Save(city.Country);
				session.Save(city);
				tx.Commit();
 			}
 
			using(ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
 			{
				//THROW
				session.SaveOrUpdate(city);
				tx.Commit();
 			}
 
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
 			{
				Assert.IsNotNull(session.Get<City>(city.Id));
				tx.Commit();
 			}
		}
 
		protected override void OnTearDown()
		{
			using(var session = OpenSession())
			using(var tx = session.BeginTransaction())
 			{
				session.Delete("from City");
				session.Delete("from Country");
				tx.Commit();
			}
		}

 	}
 }
