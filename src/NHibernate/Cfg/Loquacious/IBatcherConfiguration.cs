using NHibernate.AdoNet;
namespace NHibernate.Cfg.Loquacious
{
	public interface IBatcherConfiguration
	{
		IBatcherConfiguration Trough<TBatcher>() where TBatcher : IBatcherFactory;
		IDbIntegrationConfiguration Each(short batchSize);
	}
}