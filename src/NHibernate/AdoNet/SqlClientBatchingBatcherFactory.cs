using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class SqlClientBatchingBatcherFactory : IBatcherFactory
	{
        public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor, ISessionImplementor session)
		{
			return new SqlClientBatchingBatcher(connectionManager, interceptor, session);
		}
	}
}