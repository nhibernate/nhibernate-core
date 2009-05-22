using System.Data;

namespace NHibernate.Engine.Transaction
{
	/// <summary>
	/// Represents work that needs to be performed in a manner
	/// which isolates it from any current application unit of
	/// work transaction.
	/// </summary>
	public interface IIsolatedWork
	{
		/// <summary>
		/// Perform the actual work to be done.
		/// </summary>
		/// <param name="connection">The ADP connection to use.</param>
		void DoWork(IDbConnection connection, IDbTransaction transaction);

		// 2009-05-04 Another time we need a TransactionManager to manage isolated
		// work for a given connection.
	}
}