using NHibernate.Engine;
using NHibernate.SqlCommand;
using log4net;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
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
	}
}