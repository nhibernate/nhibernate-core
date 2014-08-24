using System;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// This is used as a marker interface for the different 
	/// transaction context required for each session
	/// </summary>
	public interface ITransactionContext : IDisposable
	{
		bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }
	}
}