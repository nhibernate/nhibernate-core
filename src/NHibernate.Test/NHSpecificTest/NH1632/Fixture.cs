using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1632
{
	using System.Transactions;
	using Cache;
	using Cfg;
	using Engine;
	using Id;

	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1632"; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration
				.SetProperty(Environment.UseSecondLevelCache, "true")
				.SetProperty(Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName);
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void When_using_DTC_HiLo_knows_to_create_isolated_DTC_transaction()
		{
			if (!Dialect.SupportsConcurrentWritingConnections)
				Assert.Ignore(Dialect.GetType().Name + " does not support concurrent writing connections, can not isolate work.");

			object scalar1, scalar2;

			using (var session = Sfi.OpenSession())
			using (var command = session.Connection.CreateCommand())
			{
				command.CommandText = "select next_hi from hibernate_unique_key";
				scalar1 = command.ExecuteScalar();
			}

			using (new TransactionScope())
			{
				var generator = Sfi.GetIdentifierGenerator(typeof(Person).FullName);
				Assert.That(generator, Is.InstanceOf<TableHiLoGenerator>());

				using (var session = OpenSession())
				{
					// Force connection acquisition for having it enlisted.
					Assert.That(session.Connection.State, Is.EqualTo(ConnectionState.Open));
					generator.Generate((ISessionImplementor)session, new Person());
				}

				// intentionally dispose without committing
			}

			using (var session = Sfi.OpenSession())
			using (var command = session.Connection.CreateCommand())
			{
				command.CommandText = "select next_hi from hibernate_unique_key";
				scalar2 = command.ExecuteScalar();
			}

			Assert.AreNotEqual(scalar1, scalar2, "HiLo must run with in its own transaction");
		}

		[Test]
		public void Dispose_session_inside_transaction_scope()
		{
			ISession s;

			using (var tx = new TransactionScope())
			{
				using (s = Sfi.OpenSession())
				{

				}
				tx.Complete();
			}

			Assert.IsFalse(s.IsOpen);
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void When_commiting_items_in_DTC_transaction_will_add_items_to_2nd_level_cache()
		{
			using (var tx = new TransactionScope())
			{
				using (var s = Sfi.OpenSession())
				{
					s.Save(new Nums { ID = 29, NumA = 1, NumB = 3 });
				}
				tx.Complete();
			}
			try
			{

				using (var tx = new TransactionScope())
				{
					using (var s = OpenSession())
					{
						var nums = s.Load<Nums>(29);
						Assert.AreEqual(1, nums.NumA);
						Assert.AreEqual(3, nums.NumB);
					}
					tx.Complete();
				}

				//closing the connection to ensure we can't really use it.
				var connection = Sfi.ConnectionProvider.GetConnection();
				Sfi.ConnectionProvider.CloseConnection(connection);

				// The session is supposed to succeed because the second level cache should have the
				// entity to load, allowing the session to not use the connection at all.
				// Will fail if a transaction manager tries to enlist user supplied connection. Do
				// not add a transaction scope below.
				using (var s = Sfi.WithOptions().Connection(connection).OpenSession())
				{
					Nums nums = null;
					Assert.DoesNotThrow(() => nums = s.Load<Nums>(29), "Failed loading entity from second level cache.");
					Assert.AreEqual(1, nums.NumA);
					Assert.AreEqual(3, nums.NumB);
				}
			}
			finally
			{
				using (var s = OpenSession())
				using (var tx = s.BeginTransaction())
				{
					var nums = s.Load<Nums>(29);
					s.Delete(nums);
					tx.Commit();
				}
			}
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void When_committing_transaction_scope_will_commit_transaction()
		{
			object id;
			using (var tx = new TransactionScope())
			{
				using (ISession s = Sfi.OpenSession())
				{
					id = s.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
				}
				tx.Complete();
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Nums nums = s.Get<Nums>(id);
				Assert.IsNotNull(nums);
				s.Delete(nums);

				tx.Commit();
			}
		}

		[Test]
		public void Will_not_save_when_flush_mode_is_never()
		{
			object id;
			using (var tx = new TransactionScope())
			{
				using (ISession s = Sfi.OpenSession())
				{
					s.FlushMode = FlushMode.Manual;
					id = s.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
				}
				tx.Complete();
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Nums nums = s.Get<Nums>(id);
				Assert.IsNull(nums);
				tx.Commit();
			}
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void When_using_two_sessions_with_explicit_flush()
		{
			if (!Dialect.SupportsConcurrentWritingConnectionsInSameTransaction)
				Assert.Ignore(Dialect.GetType().Name + " does not support concurrent connections in same transaction.");
			if (!Dialect.SupportsDistributedTransactions)
				Assert.Ignore(Dialect.GetType().Name + " does not support distributed transactions.");

			object id1, id2;
			using (var tx = new TransactionScope())
			{
				using (ISession s1 = Sfi.OpenSession())
				using (ISession s2 = Sfi.OpenSession())
				{

					id1 = s1.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
					s1.Flush();

					id2 = s2.Save(new Nums { NumA = 1, NumB = 2, ID = 6 });
					s2.Flush();

					tx.Complete();
				}
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Nums nums = s.Get<Nums>(id1);
				Assert.IsNotNull(nums);
				s.Delete(nums);

				nums = s.Get<Nums>(id2);
				Assert.IsNotNull(nums);
				s.Delete(nums);

				tx.Commit();
			}
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void When_using_two_sessions()
		{
			if (!Dialect.SupportsConcurrentWritingConnectionsInSameTransaction)
				Assert.Ignore(Dialect.GetType().Name + " does not support concurrent connections in same transaction.");
			if (!Dialect.SupportsDistributedTransactions)
				Assert.Ignore(Dialect.GetType().Name + " does not support distributed transactions.");

			object id1, id2;
			using (var tx = new TransactionScope())
			{
				using (ISession s1 = Sfi.OpenSession())
				using (ISession s2 = Sfi.OpenSession())
				{

					id1 = s1.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });

					id2 = s2.Save(new Nums { NumA = 1, NumB = 2, ID = 6 });

					tx.Complete();
				}
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Nums nums = s.Get<Nums>(id1);
				Assert.IsNotNull(nums);
				s.Delete(nums);

				nums = s.Get<Nums>(id2);
				Assert.IsNotNull(nums);
				s.Delete(nums);

				tx.Commit();
			}
		}
	}
}
