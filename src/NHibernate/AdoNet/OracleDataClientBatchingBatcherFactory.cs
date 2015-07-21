using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class OracleDataClientBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new OracleDataClientBatchingBatcher(connectionManager, interceptor);
		}
	}
}