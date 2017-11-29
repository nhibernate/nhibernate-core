using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1226
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					var bank = new Bank { Code = "01234" };
					session.Save(bank);

					var account = new Account { Bank = bank };
					session.Save(account);

					var account2 = new Account { Bank = bank };
					session.Save(account2);

					tx.Commit();
				}
			}
		}

		[Test]
		public void BankShouldBeJoinFetched()
		{
			using (var session = OpenSession())
			{
				var wasStatisticsEnabled = session.SessionFactory.Statistics.IsStatisticsEnabled;
				session.SessionFactory.Statistics.IsStatisticsEnabled = true;

				long statementCount;

				using (var tx = session.BeginTransaction())
				{
					// Bug only occurs if the Banks are already in the session cache.
					var preloadedBanks = session.CreateQuery("from Bank").List<Bank>();

					var countBeforeQuery = session.SessionFactory.Statistics.PrepareStatementCount;

					Console.WriteLine("Query: -------------------------------------------------------");

					var accounts = session.CreateQuery("from Account a left join fetch a.Bank").List<Account>();
					IList<Bank> associatedBanks = accounts.Select(x => x.Bank).ToList();

					var countAfterQuery = session.SessionFactory.Statistics.PrepareStatementCount;
					statementCount = countAfterQuery - countBeforeQuery;

					Console.WriteLine("End ----------------------------------------------------------");

					tx.Commit();
				}

				session.SessionFactory.Statistics.IsStatisticsEnabled = wasStatisticsEnabled;

				Assert.That(statementCount, Is.EqualTo(1));
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					session.Delete("from Account");
					session.Delete("from Bank");
					tx.Commit();
				}
			}
		}
	}
}
