using System;
using NHibernate.Action;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using log4net;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	[CLSCompliant(false)]
	public abstract class AbstractStatementExecutor : IStatementExecutor
	{
		private readonly ILog log;

		protected AbstractStatementExecutor(HqlSqlWalker walker, ILog log)
		{
			Walker = walker;
			this.log = log;
		}

		protected HqlSqlWalker Walker { get; private set; }

		public abstract SqlString[] SqlStatements{get;}

		public abstract int Execute(QueryParameters parameters, ISessionImplementor session);

		protected abstract IQueryable[] AffectedQueryables { get; }

		protected virtual void CoordinateSharedCacheCleanup(ISessionImplementor session)
		{
			var action = new BulkOperationCleanupAction(session, AffectedQueryables);

			action.Init();

			if (session.IsEventSource)
			{
				((IEventSource)session).ActionQueue.AddAction(action);
			}
		}
	}
}