using System;

namespace NHibernate.AdoNet
{
	/// <summary> 
	/// A BatcherFactory implementation which constructs Batcher instances
	/// capable of actually performing batch operations. 
	/// </summary>

	public class BatchingBatcherFactory: IBatcherFactory
	{
		#region IBatcherFactory Members

		public Engine.IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			throw new Exception("No default Batcher is available.");
		}

		#endregion
	}
}
