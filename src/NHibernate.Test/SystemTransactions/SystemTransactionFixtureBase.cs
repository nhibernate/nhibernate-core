using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Test.TransactionTest;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	public abstract class SystemTransactionFixtureBase : TransactionFixtureBase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> factory.ConnectionProvider.Driver.SupportsSystemTransactions && base.AppliesTo(factory);

		protected abstract bool UseConnectionOnSystemTransactionPrepare { get; }

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration
				.SetProperty(
					Cfg.Environment.UseConnectionOnSystemTransactionPrepare,
					UseConnectionOnSystemTransactionPrepare.ToString());
		}

		protected void IgnoreIfUnsupported(bool explicitFlush)
		{
			Assume.That(
				new[] { explicitFlush, UseConnectionOnSystemTransactionPrepare },
				Has.Some.EqualTo(true),
				"Implicit flush cannot work without using connection from system transaction prepare phase");
		}

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

		public class TransactionCompleteUsingConnectionInterceptor : EmptyInterceptor
		{
			private ISession _session;

			public Exception BeforeException { get; private set; }
			public Exception AfterException { get; private set; }

			public override void SetSession(ISession session)
			{
				_session = session;
			}

			public override void BeforeTransactionCompletion(ITransaction tx)
			{
				try
				{
					// Simulate an action causing a connection usage.
					_session.Connection.ToString();
				}
				catch (Exception ex)
				{
					BeforeException = ex;
				}
			}

			public override void AfterTransactionCompletion(ITransaction tx)
			{
				try
				{
					// Simulate an action causing a connection usage.
					_session.Connection.ToString();
				}
				catch (Exception ex)
				{
					AfterException = ex;
				}
			}
		}
	}
}