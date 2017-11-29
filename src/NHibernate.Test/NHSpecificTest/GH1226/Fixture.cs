namespace NHibernate.Test.NHSpecificTest.GH1226
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using NUnit.Framework;

   [TestFixture]
   public class Fixture : BugTestCase {
      protected override void OnSetUp() {
         base.OnSetUp();

         using (var session = OpenSession()) {
            using (var tx = session.BeginTransaction()) {
               Bank bank = new Bank();
					bank.Id = Guid.NewGuid();
			   bank.Code = "01234";
               session.Save(bank);

				Account account = new Account();
					account.Id = Guid.NewGuid();
					account.Bank = bank;
               session.Save(account);

               Account account2 = new Account();
               account2.Bank = bank;
               session.Save(account2);

               tx.Commit();
            }
         }
      }

      [Test]
      public void BankShouldBeJoinFetched() {
         using (var session = OpenSession()) {
            bool wasStatisticsEnabled = session.SessionFactory.Statistics.IsStatisticsEnabled;
            session.SessionFactory.Statistics.IsStatisticsEnabled = true;

            long statementCount;

            using (var tx = session.BeginTransaction()) {
               // Bug only occurs if the Banks are already in the session cache.
				IList<Bank> preloadedBanks = session.CreateQuery("from Bank").List<Bank>();

               long countBeforeQuery = session.SessionFactory.Statistics.PrepareStatementCount;

               Console.WriteLine("Query: -------------------------------------------------------");

               IList<Account> accounts = session.CreateQuery("from Account a left join fetch a.Bank").List<Account>();
               IList<Bank> associatedBanks = accounts.Select(x => x.Bank).ToList();

               long countAfterQuery = session.SessionFactory.Statistics.PrepareStatementCount;
               statementCount = countAfterQuery - countBeforeQuery;

               Console.WriteLine("End ----------------------------------------------------------");

               tx.Commit();
            }

            session.SessionFactory.Statistics.IsStatisticsEnabled = wasStatisticsEnabled;

            Assert.AreEqual(1, statementCount);
         }
      }

      protected override void OnTearDown() {
         base.OnTearDown();

         using (var session = OpenSession()) {
            using (var tx = session.BeginTransaction()) {
               session.Delete("from Account");
               session.Delete("from Bank");
               tx.Commit();
            }
         }
      }
   }
}
