namespace NHibernate.Shards.Threading.Exception
{
	/// <summary>
	/// Exception indicating that the result of a value-producing task,
	/// such as a {@link FutureTask}, cannot be retrieved because the task
	/// was cancelled.
	/// </summary>
	public class CancellationException : IllegalStateException
	{
		public CancellationException()
		{
		}

		public CancellationException(string message) : base(message)
		{
		}

		public CancellationException(string message, System.Exception innerException) : base(message, innerException)
		{
		}
	}
}