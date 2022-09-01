using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Builds <see cref="IUniqueEntityLoader"/> instances capable of dynamically building
	/// its batch-fetch SQL based on the actual number of entity ids waiting to be fetched.
	/// </summary>
	public class DynamicBatchingEntityLoaderBuilder : BatchingEntityLoaderBuilder
	{
		protected override IUniqueEntityLoader BuildBatchingLoader(IOuterJoinLoadable persister, int batchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return new DynamicBatchingEntityLoader(persister, batchSize, lockMode, factory, enabledFilters);
		}
	}
}
