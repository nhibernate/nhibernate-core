using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Default batching builder. See <see cref="BatchFetchStyle.Legacy"/>
	/// </summary>
	public class LegacyBatchingEntityLoaderBuilder : BatchingEntityLoaderBuilder
	{
		protected override IUniqueEntityLoader BuildBatchingLoader(IOuterJoinLoadable persister, int batchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return BatchingEntityLoader.CreateBatchingEntityLoader(persister, batchSize, lockMode, factory, enabledFilters);
		}
	}
}
