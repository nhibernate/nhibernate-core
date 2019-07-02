using BenchmarkDotNet.Attributes;

namespace NHibernate.Benchmark
{
	[MemoryDiagnoser]
	public class Session : NorthwindBenchmarkCase
	{
		[Benchmark]
		public ISessionFactory BuildSessionFactory()
		{
			return Configuration.BuildSessionFactory();
		}

		[Benchmark]
		public ISession CreateSession()
		{
			return SessionFactory.OpenSession();
		}
	}
}
