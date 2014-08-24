using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	/// <summary> 
	/// A BatcherFactory implementation which constructs Batcher instances
	/// that do not perform batch operations. 
	/// </summary>
	public class NonBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new NonBatchingBatcher(connectionManager, interceptor);
		}
	}
}
