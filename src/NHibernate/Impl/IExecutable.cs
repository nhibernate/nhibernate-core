namespace NHibernate.Impl
{
	/// <summary>
	/// An action that <see cref="ISession"/> can Execute during a
	/// <c>Flush</c>.
	/// </summary>
	internal interface IExecutable
	{
		/// <summary>
		/// 
		/// </summary>
		void BeforeExecutions();

		/// <summary>
		/// Execute the action required to write changes to the database.
		/// </summary>
		void Execute();

		/// <summary>
		/// Does the executable have an AfterTransactionCompletion process
		/// </summary>
		bool HasAfterTransactionCompletion { get; }

		/// <summary>
		/// Called after the Transaction has been completed.
		/// </summary>
		/// <param name="success"></param>
		/// <remarks>
		/// Actions should make sure that the Cache is notified about
		/// what just happened.
		/// </remarks>
		void AfterTransactionCompletion(bool success);

		/// <summary>
		/// The spaces (tables) that are affectd by this Executable action.
		/// </summary>
		/// <remarks>
		/// This is used to determine if the ISession needs to be flushed before
		/// a query is executed so stale data is not returned.
		/// </remarks>
		object[] PropertySpaces { get; }
	}
}