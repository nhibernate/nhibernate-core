using System;
using NHibernate.Engine;
using NHibernate.Test.TransactionTest;

namespace NHibernate.Test.SystemTransactions
{
	public abstract class SystemTransactionFixtureBase : TransactionFixtureBase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> factory.ConnectionProvider.Driver.SupportsSystemTransactions && base.AppliesTo(factory);

		public class AfterTransactionWaitingInterceptor : EmptyInterceptor
		{
			private ISession _session;

			public Exception Exception { get; private set; }

			public override void SetSession(ISession session)
			{
				_session = session;
			}

			public override void AfterTransactionCompletion(ITransaction tx)
			{
				try
				{
					// Simulate an action causing a Wait
					_session.GetSessionImplementation().TransactionContext?.Wait();
				}
				catch (Exception ex)
				{
					Exception = ex;
					throw;
				}
			}
		}
	}
}