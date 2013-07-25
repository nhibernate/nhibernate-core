using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class MySqlClientBatchingBatcherFactory : IBatcherFactory
	{
        public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor, ISessionImplementor session)
		{
			return new MySqlClientBatchingBatcher(connectionManager, interceptor, session);
		}
	}
}