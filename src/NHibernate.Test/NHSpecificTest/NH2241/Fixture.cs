using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2241
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var country = new Country { CountryCode = "SE", CountryName = "Sweden" };
				session.Save(country);
				tran.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Country");
				session.Delete("from User");
				tran.Commit();
			}
		}

		[Test]
		public void CanInsertUsingStatelessEvenWhenAssociatedEntityHasCacheStategy()
		{
			using (var ss = Sfi.OpenStatelessSession())
			using (var tx = ss.BeginTransaction())
			{
				var user = new User();
				user.Country = new Country { CountryCode = "SE", CountryName = "Sweden" };

				ss.Insert(user);
				tx.Commit();
			}
		}
	}
}
