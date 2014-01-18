using System;

namespace NHibernate.Transaction
{
	public class AfterTransactionCompletes : ISynchronization
	{
		#region Fields

		private readonly Action<bool> _whenCompleted;

		#endregion

		#region Constructors/Destructors

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
