using System;
using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.NH1082
{
	// Since v5.2
	[Obsolete]
	public class SynchronizationThatThrowsExceptionAtBeforeTransactionCompletion : ISynchronization
	{
		public void BeforeCompletion()
		{
			throw new BadException();
		}

		public void AfterCompletion(bool success)
		{
		}
	}
}
