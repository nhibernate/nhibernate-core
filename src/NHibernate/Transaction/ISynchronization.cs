using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace NHibernate.Transaction
{
	// Since v5.2
	/// <summary>
	/// A mimic to the javax.transaction.Synchronization callback to enable <see cref="ITransaction.RegisterSynchronization"/> 
	/// </summary>
	[Obsolete("Implement ITransactionCompletionSynchronization instead. " +
		"If implementing ITransaction, implement a 'public void " +
		"RegisterSynchronization(ITransactionCompletionSynchronization)': the TransactionExtensions extension " +
		"method will call it.")]
	public interface ISynchronization
	{
		void BeforeCompletion();
		void AfterCompletion(bool success);
	}
}
