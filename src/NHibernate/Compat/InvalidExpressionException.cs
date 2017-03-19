#if NETSTANDARD
// ReSharper disable once CheckNamespace
namespace System.Data
{
	public class InvalidExpressionException : System.Exception
	{
		public InvalidExpressionException()
		{
		}

		public InvalidExpressionException(string s)
		  : base(s)
		{
		}

		public InvalidExpressionException(string message, Exception innerException)
		  : base(message, innerException)
		{
		}
	}
}
#endif
