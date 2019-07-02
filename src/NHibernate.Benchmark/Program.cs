using System;
using BenchmarkDotNet.Running;

namespace NHibernate.Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<Session>();
		}
	}
}
