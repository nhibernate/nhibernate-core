using System;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Test.TransactionTest;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	public abstract class SystemTransactionFixtureBase : TransactionFixtureBase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> factory.ConnectionProvider.Driver.SupportsSystemTransactions && base.AppliesTo(factory);

		protected abstract bool UseConnectionOnSystemTransactionPrepare { get; }
		protected abstract bool AutoJoinTransaction { get; }

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration
				.SetProperty(
					Cfg.Environment.UseConnectionOnSystemTransactionPrepare,
					UseConnectionOnSystemTransactionPrepare.ToString());
		}

		protected void DisableConnectionAutoEnlist(Configuration configuration)
		{
			var connectionString = configuration.GetProperty(Cfg.Environment.ConnectionString);
			var autoEnlistmentKeyword = "Enlist";
			var autoEnlistmentKeywordPattern = autoEnlistmentKeyword;
			if (configuration.GetDerivedProperties().TryGetValue(Cfg.Environment.ConnectionDriver, out var driver) &&
				ReflectHelper.ClassForName(driver).IsMySqlDataDriver())
			{
				autoEnlistmentKeyword = "AutoEnlist";
				autoEnlistmentKeywordPattern = "Auto ?Enlist";
			}
			// Purge any previous enlist
			connectionString = Regex.Replace(
				connectionString, $"[^;\"a-zA-Z]*{autoEnlistmentKeywordPattern}=[^;\"]*", string.Empty,
				RegexOptions.IgnoreCase | RegexOptions.Multiline);
			connectionString += $";{autoEnlistmentKeyword}=false;";
			configuration.SetProperty(Cfg.Environment.ConnectionString, connectionString);
		}

		protected void IgnoreIfUnsupported(bool explicitFlush)
		{
			Assume.That(
				new[] { explicitFlush, UseConnectionOnSystemTransactionPrepare },
				Has.Some.EqualTo(true),
				"Implicit flush cannot work without using connection from system transaction prepare phase");
		}

		/// <summary>
		/// Open a session, manually enlisting it into ambient transaction if there is one.
		/// </summary>
		/// <returns>An newly opened session.</returns>
		protected override ISession OpenSession()
		{
			if (AutoJoinTransaction)
				return base.OpenSession();

			var session = Sfi.WithOptions().AutoJoinTransaction(false).OpenSession();
			if (System.Transactions.Transaction.Current != null)
				session.JoinTransaction();
			return session;
		}

		/// <summary>
		/// <c>WithOptions</c> having already set up <c>AutoJoinTransaction()</c>
		/// according to the fixture <see cref="AutoJoinTransaction"/> property.
		/// </summary>
		/// <returns>A session builder.</returns>
		protected ISessionBuilder WithOptions()
		{
			return Sfi.WithOptions().AutoJoinTransaction(AutoJoinTransaction);
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
