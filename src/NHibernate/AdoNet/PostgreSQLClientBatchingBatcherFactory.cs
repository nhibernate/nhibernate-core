using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class PostgreSQLClientBatchingBatcherFactory: IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new PostgreSQLClientBatchingBatcher(connectionManager, interceptor);
		}
	}
}
