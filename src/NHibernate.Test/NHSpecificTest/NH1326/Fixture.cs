using System;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Transaction;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1326
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1326"; }
		}


		[Test]
		[ExpectedException(typeof(InvalidOperationException),
			"Disconnect cannot be called while a transaction is in progress.")]
		public void ShouldThrowIfCallingDisconnectInsideTransaction()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Disconnect();
				tx.Commit();
			}
		}
	}
}