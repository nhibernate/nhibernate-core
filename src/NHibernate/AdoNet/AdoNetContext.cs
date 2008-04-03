using System;
using log4net;
using NHibernate.Transaction;

namespace NHibernate.AdoNet
{
	[Serializable]
	public class AdoNetContext : ConnectionManager.Callback
	{
		// TODO : make this the factory for "entity mode related" sessions;
		// also means making this the target of transaction-synch and the
		// thing that knows how to cascade things between related sessions
		//
		// At that point, perhaps this thing is a "SessionContext", and
		// ConnectionManager is a "JDBCContext"?  A "SessionContext" should
		// live in the impl package...

		private static readonly ILog log = LogManager.GetLogger(typeof (AdoNetContext));

		public interface IContext : ITransactionContext
		{
			/**
			* We cannot rely upon this method being called! It is only
			* called if we are using Hibernate Transaction API.
			*/
			ConnectionReleaseMode ConnectionReleaseMode { get; }
			bool IsAutoCloseSessionEnabled { get; }
			void AfterTransactionBegin(ITransaction tx);
			void BeforeTransactionCompletion(ITransaction tx);
			void AfterTransactionCompletion(bool success, ITransaction tx);
		}

		private IContext owner;
		private ConnectionManager connectionManager;
		[NonSerialized] private bool isTransactionCallbackRegistered;
		[NonSerialized] private ITransaction hibernateTransaction;

		#region Callback Members

		public void ConnectionOpened()
		{
			throw new NotImplementedException();
		}

		public void ConnectionCleanedUp()
		{
			throw new NotImplementedException();
		}

		public bool IsTransactionInProgress
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}