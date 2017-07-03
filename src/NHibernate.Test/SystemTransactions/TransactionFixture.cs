using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NHibernate.Linq;
using NHibernate.Test.TransactionTest;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	[TestFixture]
	public class TransactionFixture : TransactionFixtureBase
	{
		[Test]
		public void CanUseSystemTransactionsToCommit()
		{
			int identifier;
			using (var session = OpenSession())
			using (var tx = new TransactionScope())
			{
				var s = new Person();
				session.Save(s);
				identifier = s.Id;
				tx.Complete();
			}

			using (var session = OpenSession())
			using (var tx = new TransactionScope())
			{
				var w = session.Get<Person>(identifier);
				Assert.IsNotNull(w);
				// Without explicit Flush, this delays the delete to prepare phase, and test flushing from there
				// while having already acquired a connection due to the Get.
				session.Delete(w);
				tx.Complete();
			}
		}

		[Test]
		public void FlushFromTransactionAppliesToDisposedSharingSession()
		{
			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var t = new TransactionScope())
				{
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					var p4 = new Person();

					using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
						s1.Save(p1);
					using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
					{
						s2.Save(p2);
						using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder)).OpenSession())
							s3.Save(p3);
					}
					s.Save(p4);
					t.Complete();
				}
			}

			Assert.That(flushOrder, Is.EqualTo(new[] { 0, 1, 2, 3 }));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.Query<Person>().Count(), Is.EqualTo(4));
				t.Commit();
			}
		}

		[Test]
		public void FlushFromTransactionAppliesToSharingSession()
		{
			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
				using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
				using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder)).OpenSession())
				using (var t = new TransactionScope())
				{
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					var p4 = new Person();
					s1.Save(p1);
					s2.Save(p2);
					s3.Save(p3);
					s.Save(p4);
					t.Complete();
				}
			}

			Assert.That(flushOrder, Is.EqualTo(new[] { 0, 1, 2, 3 }));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.Query<Person>().Count(), Is.EqualTo(4));
				t.Commit();
			}
		}
	}
}