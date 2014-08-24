using System;

namespace NHibernate.Transaction
{
	public class AfterTransactionCompletes : ISynchronization
	{
		#region Fields

		private readonly Action<bool> _whenCompleted;

		#endregion

		#region Constructors/Destructors

		/// <summary>
		/// Create an AfterTransactionCompletes that will execute the given delegate
		/// when the transaction is completed. The action delegate will receive
		/// the value 'true' if the transaction was completed successfully.
		/// </summary>
		/// <param name="whenCompleted"></param>
		public AfterTransactionCompletes(Action<bool> whenCompleted)
		{
			_whenCompleted = whenCompleted;
		}

		#endregion

		#region ISynchronization Members

		public void BeforeCompletion()
		{
		}

		public void AfterCompletion(bool success)
		{
			_whenCompleted(success);
		}

		#endregion
	}
}
