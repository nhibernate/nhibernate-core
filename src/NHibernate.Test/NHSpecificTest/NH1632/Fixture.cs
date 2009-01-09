using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.NHSpecificTest.NH1632
{
	using System.Transactions;
	using Cache;
	using Cfg;

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
				.SetProperty(Environment.CacheProvider, typeof (HashtableCacheProvider).AssemblyQualifiedName);
		}

		[Test]
		public void Dispose_session_inside_transaction_scope()
		{
			ISession s;

			using (var tx = new TransactionScope())
			{
				using (s = sessions.OpenSession())
				{

				}
				tx.Complete();
			}

			Assert.IsFalse(s.IsOpen);
		}


		[Test]
		public void When_commiting_items_in_DTC_transaction_will_add_items_to_2nd_level_cache()
		{
			using (var tx = new TransactionScope())
			{
				using (var s = sessions.OpenSession())
				{
					s.Save(new Nums {ID = 29, NumA = 1, NumB = 3});
				}
				tx.Complete();
			}

			using (var tx = new TransactionScope())
			{
				using (var s = sessions.OpenSession())
				{
					var nums = s.Load<Nums>(29);
					Assert.AreEqual(1, nums.NumA);
					Assert.AreEqual(3, nums.NumB);
				}
				tx.Complete();
			}

			//closing the connection to ensure we can't really use it.
			var connection = sessions.ConnectionProvider.GetConnection();
			sessions.ConnectionProvider.CloseConnection(connection);

			using (var tx = new TransactionScope())
			{
				using (var s = sessions.OpenSession(connection))
				{
					var nums = s.Load<Nums>(29);
					Assert.AreEqual(1, nums.NumA);
					Assert.AreEqual(3, nums.NumB);
				}
				tx.Complete();
			}

			using (var s = sessions.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var nums = s.Load<Nums>(29);
				s.Delete(nums);
				tx.Commit();
			}
		}

		[Test]
		public void When_committing_transaction_scope_will_commit_transaction()
		{
			object id;
			using (var tx = new TransactionScope())
			{
				using (ISession s = sessions.OpenSession())
				{
					id = s.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
				}
				tx.Complete();
			}

			using (ISession s = sessions.OpenSession())
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
				using (ISession s = sessions.OpenSession())
				{
					s.FlushMode = FlushMode.Never;
					id = s.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
				}
				tx.Complete();
			}

			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Nums nums = s.Get<Nums>(id);
				Assert.IsNull(nums);
				tx.Commit();
			}
		}

		[Test]
		public void When_using_two_sessions_with_explicit_flush()
		{
			object id1, id2;
			using (var tx = new TransactionScope())
			{
				using (ISession s1 = sessions.OpenSession())
				using (ISession s2 = sessions.OpenSession())
				{

					id1 = s1.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });
					s1.Flush();

					id2 = s2.Save(new Nums { NumA = 1, NumB = 2, ID = 6 });
					s2.Flush();

					tx.Complete();
				}
			}

			using (ISession s = sessions.OpenSession())
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
		public void When_using_two_sessions()
		{
			object id1, id2;
			using (var tx = new TransactionScope())
			{
				using (ISession s1 = sessions.OpenSession())
				using (ISession s2 = sessions.OpenSession())
				{

					id1 = s1.Save(new Nums { NumA = 1, NumB = 2, ID = 5 });

					id2 = s2.Save(new Nums { NumA = 1, NumB = 2, ID = 6 });

					tx.Complete();
				}
			}

			using (ISession s = sessions.OpenSession())
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
