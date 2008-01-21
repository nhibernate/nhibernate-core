using System.Collections.Generic;
using NHibernate.Shards.Strategy.Exit;

namespace NHibernate.Shards.Strategy.Access
{
	/// <summary>
	/// Invokes the given operation on the given shards in parallel.
	/// </summary>
	public class ParallelShardAccessStrategy : IShardAccessStrategy
	{
		#region IShardAccessStrategy Members

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="shards"></param>
		/// <param name="operation"></param>
		/// <param name="exitStrategy"></param>
		/// <param name="exitOperationsCollector"></param>
		/// <returns></returns>
		public T Apply<T>(IList<IShard> shards,
		                  IShardOperation<T> operation,
		                  IExitStrategy<T> exitStrategy,
		                  IExitOperationsCollector exitOperationsCollector)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}