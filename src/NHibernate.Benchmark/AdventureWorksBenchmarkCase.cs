using NHibernate.Benchmark.Database.AdventureWorks.Entities;
using NHibernate.Cfg;

namespace NHibernate.Benchmark
{
	public class AdventureWorksBenchmarkCase : BenchmarkCase
	{
		protected override string[] Mappings => new[]
		{
			"Database.AdventureWorks.Mappings.BusinessEntity.hbm.xml"
		};

		protected override void AddConfigurationResource(Configuration configuration, string file)
		{
			configuration.AddResource(typeof(Program).Namespace + "." + file, typeof(BusinessEntity).Assembly);
		}
	}
}
