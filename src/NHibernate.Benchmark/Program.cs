using BenchmarkDotNet.Running;

namespace NHibernate.Benchmark
{
	class Program
	{
		static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
	}
}
