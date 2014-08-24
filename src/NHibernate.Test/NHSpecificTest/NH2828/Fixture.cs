using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2828
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void WhenPersistShouldNotFetchUninitializedCollection()
		{
			var companyId = CreateScenario();

			//Now in a second transaction i remove the address and persist Company: for a cascade option the Address will be removed
			using (var sl = new SqlLogSpy())
			{
				using (ISession session = sessions.OpenSession())
				{
					using (ITransaction tx = session.BeginTransaction())
					{
						var company = session.Get<Company>(companyId);
						company.Addresses.Count().Should().Be.EqualTo(1);
						company.RemoveAddress(company.Addresses.First()).Should().Be.EqualTo(true);

						//now this company will be saved and deleting the address.
						//BUT it should not try to load the BanckAccound collection!
						session.Persist(company);
						tx.Commit();
					}
				}
				var wholeMessage = sl.GetWholeLog();
				wholeMessage.Should().Not.Contain("BankAccount");
			}

			Cleanup(companyId);
		}

		private void Cleanup(Guid companyId)
		{
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete(session.Get<Company>(companyId));
					tx.Commit();
				}
			}
		}

		private Guid CreateScenario()
		{
			var company = new Company() {Name = "Company test"};
			var address = new Address() {Name = "Address test"};
			var bankAccount = new BankAccount() {Name = "Bank test"};
			company.AddAddress(address);
			company.AddBank(bankAccount);
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Persist(company);
					tx.Commit();
				}
			}
			return company.Id;
		}
	}
}
