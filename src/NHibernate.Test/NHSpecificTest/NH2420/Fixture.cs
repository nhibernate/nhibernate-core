using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2420
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH2420"; }
		}
		
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return (dialect is NHibernate.Dialect.MsSql2008Dialect || dialect is NHibernate.Dialect.MsSql2005Dialect);
		}

		[Test]
		[Explicit("Requires the Microsoft DTC service. Also, the exception thrown by this test when it fails is on a ThreadPool thread which is not visible to NUnit. So although the test accurately reproduces the issue, it passes anyway.")]
		public void Bug()
		{
			string connectionString = cfg.GetProperty("connection.connection_string");
			int id = -1;
			
			using (TransactionScope ts = new TransactionScope())
			{
				// Enlisting DummyEnlistment as a durable resource manager will start
				// a DTC transaction
				System.Transactions.Transaction.Current.EnlistDurable(
					DummyEnlistment.Id,
					new DummyEnlistment(),
					EnlistmentOptions.None);
				
				using (IDbConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					using (ISession s = Sfi.OpenSession(connection))
					{
						id = (int)s.Save(new MyTable() { String = "hello!" });
					}
					connection.Close();
				}
				
				// Prior to the patch, an InvalidOperationException exception would occur in the
				// TransactionCompleted delegate at this point with the message, "Disconnect cannot
				// be called while a transaction is in progress". Although the exception can be
				// seen reported in the IDE, NUnit fails to see it, and the test passes. The
				// TransactionCompleted event fires *after* the transaction is committed and so
				// it doesn't affect the success of the transaction
				ts.Complete();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from MyTable");
				s.Flush();
			}
		}
	}
}