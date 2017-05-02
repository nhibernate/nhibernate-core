using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Cfg;
using NHibernate.Util;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class AggressiveReleaseTest : ConnectionManagementTestCase
	{
		protected override void Configure(Configuration cfg)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.ReleaseConnections, "after_transaction");
			//cfg.SetProperty(Environment.ConnectionProvider, typeof(DummyConnectionProvider).AssemblyQualifiedName);
			//cfg.SetProperty(Environment.GenerateStatistics, "true");
			cfg.SetProperty(Environment.BatchSize, "0");
		}

		protected override ISession GetSessionUnderTest()
		{
			return OpenSession();
		}

		protected void Reconnect(ISession session)
		{
			session.Reconnect();
		}

		protected override void Prepare()
		{
			//DummyTransactionManager.INSTANCE.Begin();
		}

		protected override void Done()
		{
			//DummyTransactionManager.INSTANCE.Commit();
		}

		// Some additional tests specifically for the aggressive-Release functionality...

		[Test]
		public void SerializationOnAfterStatementAggressiveRelease()
		{
			Prepare();
			ISession s = GetSessionUnderTest();
			Silly silly = new Silly("silly");
			s.Save(silly);

			// this should cause the CM to obtain a connection, and then Release it
			s.Flush();

			// We should be able to serialize the session at this point...
			SerializationHelper.Serialize(s);

			s.Delete(silly);
			s.Flush();

			Release(s);
			Done();
		}

		[Test]
		public void SerializationFailsOnAfterStatementAggressiveReleaseWithOpenResources()
		{
			Prepare();
			ISession s = GetSessionUnderTest();

			Silly silly = new Silly("silly");
			s.Save(silly);

			// this should cause the CM to obtain a connection, and then Release it
			s.Flush();

			// both scroll() and iterate() cause the batcher to hold on
			// to resources, which should make aggresive-Release not Release
			// the connection (and thus cause serialization to fail)
			IEnumerable en = s.CreateQuery("from Silly").Enumerable();

			try
			{
				SerializationHelper.Serialize(s);
				Assert.Fail(
					"Serialization allowed on connected session; or aggressive Release released connection with open resources");
			}
			catch (InvalidOperationException)
			{
				// expected behavior
			}

			// Closing the ScrollableResults does currently force the batcher to
			// aggressively Release the connection
			NHibernateUtil.Close(en);
			SerializationHelper.Serialize(s);

			s.Delete(silly);
			s.Flush();

			Release(s);
			Done();
		}

		[Test]
		public void QueryIteration()
		{
			Prepare();
			ISession s = GetSessionUnderTest();
			Silly silly = new Silly("silly");
			s.Save(silly);
			s.Flush();

			IEnumerable en = s.CreateQuery("from Silly").Enumerable();
			IEnumerator itr = en.GetEnumerator();
			Assert.IsTrue(itr.MoveNext());
			Silly silly2 = (Silly) itr.Current;
			Assert.AreEqual(silly, silly2);
			NHibernateUtil.Close(itr);

			itr = s.CreateQuery("from Silly").Enumerable().GetEnumerator();
			IEnumerator itr2 = s.CreateQuery("from Silly where name = 'silly'").Enumerable().GetEnumerator();

			Assert.IsTrue(itr.MoveNext());
			Assert.AreEqual(silly, itr.Current);
			Assert.IsTrue(itr2.MoveNext());
			Assert.AreEqual(silly, itr2.Current);

			NHibernateUtil.Close(itr);
			NHibernateUtil.Close(itr2);

			s.Delete(silly);
			s.Flush();

			Release(s);
			Done();
		}

		//[Test]
		//public void QueryScrolling()
		//{
		//    Prepare();
		//    ISession s = GetSessionUnderTest();
		//    Silly silly = new Silly("silly");
		//    s.Save(silly);
		//    s.Flush();

		//    ScrollableResults sr = s.CreateQuery("from Silly").scroll();
		//    Assert.IsTrue(sr.next());
		//    Silly silly2 = (Silly) sr.get(0);
		//    Assert.AreEqual(silly, silly2);
		//    sr.Close();

		//    sr = s.CreateQuery("from Silly").Scroll();
		//    ScrollableResults sr2 = s.CreateQuery("from Silly where name = 'silly'").Scroll();

		//    Assert.IsTrue(sr.next());
		//    Assert.AreEqual(silly, sr.get(0));
		//    Assert.IsTrue(sr2.next());
		//    Assert.AreEqual(silly, sr2.get(0));

		//    sr.Close();
		//    sr2.Close();

		//    s.Delete(silly);
		//    s.Flush();

		//    Release(s);
		//    Done();
		//}

		[Test]
		public void SuppliedConnection()
		{
			Prepare();

			using (var originalConnection = sessions.ConnectionProvider.GetConnection())
			using (var session = sessions.WithOptions().Connection(originalConnection).OpenSession())
			{
				var silly = new Silly("silly");
				session.Save(silly);

				// this will cause the connection manager to cycle through the aggressive Release logic;
				// it should not Release the connection since we explicitly supplied it ourselves.
				session.Flush();

				Assert.IsTrue(originalConnection == session.Connection, "Different connections");

				session.Delete(silly);
				session.Flush();

				Release(session);
				originalConnection.Close();
			}
			Done();
		}

		// TODO
		//[Test]
		//public void BorrowedConnections()
		//{
		//    Prepare();
		//    ISession s = GetSessionUnderTest();

		//    DbConnection conn = s.Connection;
		//    Assert.IsTrue(((SessionImpl) s).ConnectionManager.HasBorrowedConnection);
		//    conn.Close();
		//    Assert.IsFalse(((SessionImpl) s).ConnectionManager.HasBorrowedConnection);

		//    Release(s);
		//    Done();
		//}

		[Test]
		public void ConnectionMaintanenceDuringFlush()
		{
			Prepare();
			ISession s = GetSessionUnderTest();
			s.BeginTransaction();

			IList<Silly> entities = new List<Silly>();
			for (int i = 0; i < 10; i++)
			{
				Other other = new Other("other-" + i);
				Silly silly = new Silly("silly-" + i, other);
				entities.Add(silly);
				s.Save(silly);
			}
			s.Flush();

			foreach (Silly silly in entities)
			{
				silly.Name = "new-" + silly.Name;
				silly.Other.Name = "new-" + silly.Other.Name;
			}
//			long initialCount = sessions.Statistics.getConnectCount();
			s.Flush();
//			Assert.AreEqual(initialCount + 1, sessions.Statistics.getConnectCount(), "connection not maintained through Flush");

			s.Delete("from Silly");
			s.Delete("from Other");
			s.Transaction.Commit();
			Release(s);
			Done();
		}
	}
}