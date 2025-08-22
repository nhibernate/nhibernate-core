using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3474
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var e1 = new CreditCardPayment { CreditCardType = "Visa", Amount = 50 };
			session.Save(e1);

			var e2 = new ChequePayment { Bank = "CA", Amount = 32 };
			session.Save(e2);

			var e3 = new CashPayment { Amount = 18.5m };
			session.Save(e3);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			// The HQL delete does all the job inside the database without loading the entities, but it does
			// not handle delete order for avoiding violating constraints if any. Use
			// session.Delete("from System.Object");
			// instead if in need of having NHibernate ordering the deletes, but this will cause
			// loading the entities in the session.
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Polymorphic updates require support of temp tables.
			return Dialect.SupportsTemporaryTables;
		}

		[Test]
		public void PolymorphicUpdateShouldNotCommit()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var payment = session.Query<CreditCardPayment>().First();
				payment.Amount = 100;
				session.Flush();

				session.CreateQuery("update ChequePayment set Amount = 64").ExecuteUpdate();

				transaction.Rollback();
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				IPayment payment = session.Query<CreditCardPayment>().First();
				Assert.That(payment.Amount, Is.EqualTo(50m));

				payment = session.Query<ChequePayment>().First();
				Assert.That(payment.Amount, Is.EqualTo(32m));

				transaction.Commit();
			}
		}
	}
}
