using System;
using System.Collections;
using System.Threading;
using System.Transactions;
using log4net;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.DtcFailures
{
	[TestFixture]
	public class DtcFailuresFixture : TestCase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DtcFailuresFixture));

		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.DtcFailures.Mappings.hbm.xml"}; }
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
				s.Save(new Person { CreatedAt = DateTime.Today });

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
		[Description("Another action inside the transaction do the rollBack outside nh-session-scope.")]
		public void RollbackOutsideNh()
		{
			try
			{
				using (var txscope = new TransactionScope())
				{
					using (ISession s = sessions.OpenSession())
					{
						var person = new Person { CreatedAt = DateTime.Now };
						s.Save(person);
					}
					new ForceEscalationToDistributedTx(true); //will rollback tx

					txscope.Complete();
				}

				log.DebugFormat("Transaction fail.");
				Assert.Fail("Expected tx abort");
			}
			catch (TransactionAbortedException)
			{
				log.DebugFormat("Transaction aborted.");
			}
		}

		[Test]
		[Description("rollback inside nh-session-scope should not commit save and the transaction should be aborted.")]
		public void TransactionInsertWithRollBackTask()
		{
			try
			{
				using (var txscope = new TransactionScope())
				{
					using (ISession s = sessions.OpenSession())
					{
						var person = new Person {CreatedAt = DateTime.Now};
						s.Save(person);
						new ForceEscalationToDistributedTx(true); //will rollback tx
						person.CreatedAt = DateTime.Now;
						s.Update(person);
					}
					txscope.Complete();
				}
				log.DebugFormat("Transaction fail.");
				Assert.Fail("Expected tx abort");
			}
			catch (TransactionAbortedException)
			{
				log.DebugFormat("Transaction aborted.");
			}
		}

		[Test, Ignore("Not fixed.")]
		[Description(@"Two session in two txscope 
(without an explicit NH transaction and without an explicit flush) 
and with a rollback in the second dtc and a ForceRollback outside nh-session-scope.")]
		public void TransactionInsertLoadWithRollBackTask()
		{
			object savedId;
			using (var txscope = new TransactionScope())
			{
				using (ISession s = sessions.OpenSession())
				{
					var person = new Person {CreatedAt = DateTime.Now};
					savedId = s.Save(person);
				}
				txscope.Complete();
			}
			try
			{
				using (var txscope = new TransactionScope())
				{
					using (ISession s = sessions.OpenSession())
					{
						var person = s.Get<Person>(savedId);
						person.CreatedAt = DateTime.Now;
						s.Update(person);
					}
					new ForceEscalationToDistributedTx(true);

					log.Debug("completing the tx scope");
					txscope.Complete();
				}
				log.Debug("Transaction fail.");
				Assert.Fail("Expected tx abort");
			}
			catch (TransactionAbortedException)
			{
				log.Debug("Transaction aborted.");
			}
			finally
			{
				using (var txscope = new TransactionScope())
				{
					using (ISession s = sessions.OpenSession())
					{
						var person = s.Get<Person>(savedId);
						s.Delete(person);
					}
					txscope.Complete();
				}
			}
		}

		private int totalCall;

		[Test, Explicit]
		public void MultiThreadedTransaction()
		{
			// Test added for NH-1709 (trying to recreate the issue... without luck)
			// If one thread break the test, you can see the result in the console.
			((Logger)log.Logger).Level = log4net.Core.Level.Debug;
			var actions = new MultiThreadRunner<object>.ExecuteAction[]
        	{
        		delegate(object o)
        			{
        				Can_roll_back_transaction();
        				totalCall++;
        			},
        		delegate(object o)
        			{
        				RollbackOutsideNh();
        				totalCall++;
        			},
        		delegate(object o)
        			{
        				TransactionInsertWithRollBackTask();
        				totalCall++;
        			},
						//delegate(object o)
						//  {
						//    TransactionInsertLoadWithRollBackTask();
						//    totalCall++;
						//  },
        	};
			var mtr = new MultiThreadRunner<object>(20, actions)
			          	{
			          	  EndTimeout	= 5000, TimeoutBetweenThreadStart = 5
			          	};
			mtr.Run(null);
			log.DebugFormat("{0} calls", totalCall);
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

		[Test]
		[Description("Open/Close a session inside a TransactionScope fails.")]
		public void NH1744()
		{
			using (var tx = new TransactionScope())
			{
				using (ISession s = sessions.OpenSession())
				{
					s.Flush();
				}

				using (ISession s = sessions.OpenSession())
				{
					s.Flush();
				}

				//and I always leave the transaction disposed without calling tx.Complete(), I let the database server to rollback all actions in this test. 
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
					log.Debug(">>>>Force Rollback<<<<<");
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