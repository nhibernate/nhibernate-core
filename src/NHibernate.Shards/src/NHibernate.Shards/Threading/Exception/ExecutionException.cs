namespace NHibernate.Shards.Threading.Exception
{
	/// <summary>
	/// Exception thrown when attempting to retrieve the result of a task
	/// that aborted by throwing an exception. This exception can be
	/// inspected using the {@link #getCause()} method.
	/// </summary>
	public class ExecutionException : System.Exception
	{
		public ExecutionException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		public ExecutionException(string message) : base(message)
		{
		}

		public ExecutionException(System.Exception ex) : this(string.Empty, ex)
		{
		}

		public ExecutionException()
		{
		}
	}
}