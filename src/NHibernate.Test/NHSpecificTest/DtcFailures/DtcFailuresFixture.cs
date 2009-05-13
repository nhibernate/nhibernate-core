using System;
using System.Collections;
using System.Threading;
using System.Transactions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.DtcFailures
{
	[TestFixture]
	public class DtcFailuresFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.DtcFailures.Mappings.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void WillNotCrashOnDtcPrepareFailure()
		{
			var tx = new TransactionScope();
			using (ISession s = sessions.OpenSession())
			{
				s.Save(new Person {CreatedAt = DateTime.MinValue // will cause SQL date failure
				                  });
			}

			new ForceEscalationToDistributedTx();

			tx.Complete();
			try
			{
				tx.Dispose();
				Assert.Fail("Expected failure");
			}
			catch (AssertionException)
			{
				throw;
			}
			catch (Exception) {}
		}

		[Test]
		public void Can_roll_back_transaction()
		{
			var tx = new TransactionScope();
			using (ISession s = sessions.OpenSession())
			{
				new ForceEscalationToDistributedTx(true); //will rollback tx
				s.Save(new Person {CreatedAt = DateTime.Today});

				tx.Complete();
			}
			try
			{
				tx.Dispose();
				Assert.Fail("Expected tx abort");
			}
			catch (TransactionAbortedException)
			{
				//expected   
			}
		}

		[Test]
		public void CanDeleteItemInDtc()
		{
			object id;
			using (var tx = new TransactionScope())
			{
				using (ISession s = sessions.OpenSession())
				{
					id = s.Save(new Person {CreatedAt = DateTime.Today});

					new ForceEscalationToDistributedTx();

					tx.Complete();
				}
			}

			using (var tx = new TransactionScope())
			{
				using (ISession s = sessions.OpenSession())
				{
					new ForceEscalationToDistributedTx();

					s.Delete(s.Get<Person>(id));

					tx.Complete();
				}
			}
		}

		public class ForceEscalationToDistributedTx : IEnlistmentNotification
		{
			private readonly bool shouldRollBack;
			private readonly int thread;

			public ForceEscalationToDistributedTx(bool shouldRollBack)
			{
				this.shouldRollBack = shouldRollBack;
				thread = Thread.CurrentThread.ManagedThreadId;
				System.Transactions.Transaction.Current.EnlistDurable(Guid.NewGuid(), this, EnlistmentOptions.None);
			}

			public ForceEscalationToDistributedTx() : this(false) {}

			public void Prepare(PreparingEnlistment preparingEnlistment)
			{
				Assert.AreNotEqual(thread, Thread.CurrentThread.ManagedThreadId);
				if (shouldRollBack)
				{
					preparingEnlistment.ForceRollback();
				}
				else
				{
					preparingEnlistment.Prepared();
				}
			}

			public void Commit(Enlistment enlistment)
			{
				enlistment.Done();
			}

			public void Rollback(Enlistment enlistment)
			{
				enlistment.Done();
			}

			public void InDoubt(Enlistment enlistment)
			{
				enlistment.Done();
			}
		}
	}
}