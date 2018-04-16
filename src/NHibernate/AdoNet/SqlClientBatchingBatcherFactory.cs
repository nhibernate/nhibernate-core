#if NETFX
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class SqlClientBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new SqlClientBatchingBatcher(connectionManager, interceptor);
		}
	}
}
#endif
