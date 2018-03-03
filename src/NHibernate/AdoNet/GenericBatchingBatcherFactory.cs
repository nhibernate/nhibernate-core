using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class GenericBatchingBatcherFactory: IBatcherFactory
	{
		private char _statementTerminator = ';';

		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new GenericBatchingBatcher(connectionManager, interceptor, _statementTerminator);
		}

		public void SetStatementTerminator(char value)
		{
			_statementTerminator = value;
		}
	}
}
