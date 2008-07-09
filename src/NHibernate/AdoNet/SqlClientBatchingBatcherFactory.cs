using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	internal class SqlClientBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new SqlClientBatchingBatcher(connectionManager, interceptor);
		}
	}
}