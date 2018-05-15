using System.Linq;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
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
			Sfi.Statistics.IsStatisticsEnabled = true;
		}

		[Test]
		public void BankShouldBeJoinFetched()
		{
			// Simple case: nothing already in session.
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var countBeforeQuery = Sfi.Statistics.PrepareStatementCount;

				var accounts = session.CreateQuery("from Account a left join fetch a.Bank").List<Account>();
				var associatedBanks = accounts.Select(x => x.Bank).ToList();
				Assert.That(associatedBanks, Has.All.Matches<object>(NHibernateUtil.IsInitialized),
				            "One bank or more was lazily loaded.");

				var countAfterQuery = Sfi.Statistics.PrepareStatementCount;
				var statementCount = countAfterQuery - countBeforeQuery;

				tx.Commit();

				Assert.That(statementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void InSessionBankShouldBeJoinFetched()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				// #1226 bug only occurs if the Banks are already in the session cache.
				session.CreateQuery("from Bank").List<Bank>();

				var countBeforeQuery = Sfi.Statistics.PrepareStatementCount;

				var accounts = session.CreateQuery("from Account a left join fetch a.Bank").List<Account>();
				var associatedBanks = accounts.Select(x => x.Bank).ToList();
				Assert.That(associatedBanks, Has.All.Matches<object>(NHibernateUtil.IsInitialized),
				            "One bank or more was lazily loaded.");

				var countAfterQuery = Sfi.Statistics.PrepareStatementCount;
				var statementCount = countAfterQuery - countBeforeQuery;

				tx.Commit();

				Assert.That(statementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void AlteredBankShouldBeJoinFetched()
		{
			using (var s1 = OpenSession())
			{
				using (var tx = s1.BeginTransaction())
				{
					// Put them all in s1 cache.
					s1.CreateQuery("from Bank").List();
					tx.Commit();
				}

				string oldCode;
				const string newCode = "12345";
				// Alter the bank code with another session.
				using (var s2 = OpenSession())
				using (var tx2 = s2.BeginTransaction())
				{
					var accounts = s2.Query<Account>().ToList();
					foreach (var account in accounts)
						account.Bank = null;
					s2.Flush();
					var bank = s2.Query<Bank>().Single();
					oldCode = bank.Code;
					bank.Code = newCode;
					s2.Flush();
					foreach (var account in accounts)
						account.Bank = bank;
					tx2.Commit();
				}

				// Check querying them with s1 is still consistent
				using (var tx = s1.BeginTransaction())
				{
					var accounts = s1.CreateQuery("from Account a left join fetch a.Bank").List<Account>();
					var associatedBanks = accounts.Select(x => x.Bank).ToList();
					Assert.That(associatedBanks, Has.All.Not.Null,
					            "One bank or more failed loading.");
					Assert.That(associatedBanks, Has.All.Matches<object>(NHibernateUtil.IsInitialized),
					            "One bank or more was lazily loaded.");
					Assert.That(associatedBanks, Has.All.Property(nameof(Bank.Code)).EqualTo(oldCode),
					            "One bank or more has no more the old code.");

					tx.Commit();
					// Do not check statements count: we are in a special case defeating the eager fetching, because
					// we have stale data in session for the bank code.
					// But check that the new code, supposed to be unknown for the session, is not cached.
					var persister = Sfi.GetEntityPersister(typeof(Bank).FullName);
					var index = ((IUniqueKeyLoadable) persister).GetPropertyIndex(nameof(Bank.Code));
					var type = persister.PropertyTypes[index];
					var euk = new EntityUniqueKey(persister.EntityName, nameof(Bank.Code), newCode, type, Sfi);
					Assert.That(s1.GetSessionImplementation().PersistenceContext.GetEntity(euk),
						Is.Null, "Found a bank associated to the new code in s1");
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from Account").ExecuteUpdate();
				session.CreateQuery("delete from Bank").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}
