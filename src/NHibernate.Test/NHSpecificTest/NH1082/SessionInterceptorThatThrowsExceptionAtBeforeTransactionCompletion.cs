using System;

namespace NHibernate.Test.NHSpecificTest.NH1082
{
	public class SessionInterceptorThatThrowsExceptionAtBeforeTransactionCompletion : EmptyInterceptor
	{
		public override void BeforeTransactionCompletion(ITransaction tx)
		{
			throw new BadException();
		}
	}

	public class BadException : Exception
	{ }
}