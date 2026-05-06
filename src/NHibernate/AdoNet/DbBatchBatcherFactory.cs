#if NET6_0_OR_GREATER
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class DbBatchBatcherFactory : IBatcherFactory
	{
		public IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new DbBatchBatcher(connectionManager, interceptor);
		}
	}
}
#endif
