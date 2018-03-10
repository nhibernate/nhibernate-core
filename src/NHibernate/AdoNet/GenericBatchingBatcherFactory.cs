using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	public class GenericBatchingBatcherFactory: IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new GenericBatchingBatcher(connectionManager, interceptor);
		}
	}
}
