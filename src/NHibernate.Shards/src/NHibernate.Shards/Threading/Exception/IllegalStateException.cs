namespace NHibernate.Shards.Threading.Exception
{
	/// <summary>
	/// Signals that a method has been invoked at an illegal or
	/// inappropriate time.
	/// </summary>
	public class IllegalStateException : System.Exception
	{
		public IllegalStateException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		public IllegalStateException(string message) : base(message)
		{
		}

		public IllegalStateException()
		{
		}
	}
}