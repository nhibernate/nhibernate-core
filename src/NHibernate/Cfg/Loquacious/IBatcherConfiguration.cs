using System;
using NHibernate.AdoNet;
namespace NHibernate.Cfg.Loquacious
{
	[Obsolete("Replaced by direct class usage")]
	public interface IBatcherConfiguration
	{
		IBatcherConfiguration Through<TBatcher>() where TBatcher : IBatcherFactory;
		IDbIntegrationConfiguration Each(short batchSize);
		IBatcherConfiguration OrderingInserts();
		IBatcherConfiguration DisablingInsertsOrdering();
	}
}
