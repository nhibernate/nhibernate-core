using System;
using System.Transactions;

namespace NHibernate.Test.NHSpecificTest.NH2420
{
	public class DummyEnlistment : IEnlistmentNotification
	{
		public static readonly Guid Id = new Guid("E2D35055-4187-4ff5-82A1-F1F161A008D0");

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
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