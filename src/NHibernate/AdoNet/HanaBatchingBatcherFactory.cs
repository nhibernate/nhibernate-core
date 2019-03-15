using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class HanaBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new HanaBatchingBatcher(connectionManager, interceptor);
		}
	}
}
