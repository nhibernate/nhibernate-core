using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// The contract for building <seealso cref="IUniqueEntityLoader"/> capable of performing batch-fetch loading.
	/// </summary>
	public abstract class BatchingEntityLoaderBuilder
	{
		/// <summary>
		/// Builds a batch-fetch capable loader based on the given persister, lock-mode, etc.
		/// </summary>
		/// <param name="persister"> The entity persister </param>
		/// <param name="batchSize"> The maximum number of ids to batch-fetch at once </param>
		/// <param name="lockMode"> The lock mode </param>
		/// <param name="factory"> The SessionFactory </param>
		/// <param name="enabledFilters"></param>
		/// <returns> The loader. </returns>
		public virtual IUniqueEntityLoader BuildLoader(IOuterJoinLoadable persister, int batchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			if (batchSize <= 1)
			{
				// no batching
				return new EntityLoader(persister, lockMode, factory, enabledFilters);
			}

			return BuildBatchingLoader(persister, batchSize, lockMode, factory, enabledFilters);
		}

		protected abstract IUniqueEntityLoader BuildBatchingLoader(IOuterJoinLoadable persister, int batchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters);
	}
}
