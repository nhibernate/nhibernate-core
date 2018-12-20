using System;
using System.Diagnostics;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	public abstract class AccessorPerformanceFixture<T> where T : new()
	{
		private IPropertyAccessor _accessor;
		private ISetter _reflectionSetter;
		private ISetter _ilSetter;
		private IGetter _reflectionGetter;
		private IGetter _ilGetter;

		protected abstract string AccessorType { get; }

		protected abstract string Path { get; }

		protected abstract object GetValue();

		[SetUp]
		public void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor(AccessorType);
			var optimize = Cfg.Environment.UseReflectionOptimizer;

			try
			{
				Cfg.Environment.UseReflectionOptimizer = false;
				_reflectionSetter = _accessor.GetSetter(typeof(T), Path);
				_reflectionGetter = _accessor.GetGetter(typeof(T), Path);

				Cfg.Environment.UseReflectionOptimizer = true;
				_ilSetter = _accessor.GetSetter(typeof(T), Path);
				_ilGetter = _accessor.GetGetter(typeof(T), Path);
			}
			finally
			{
				Cfg.Environment.UseReflectionOptimizer = optimize;
			}
		}

		[TestCase(50000)]
		[TestCase(100000)]
		[TestCase(200000)]
		[TestCase(500000)]
		public void TestGet(int iter)
		{
			var target = new T();
			var stopwatch = new Stopwatch();

			// Warm up
			TestGetter(_reflectionGetter, target, 100);
			TestGetter(_ilGetter, target, 100);

			stopwatch.Restart();
			TestGetter(_reflectionGetter, target, iter);
			stopwatch.Stop();
			Console.WriteLine($"Reflection getter total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestGetter(_ilGetter, target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IL getter total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");
		}

		[TestCase(50000)]
		[TestCase(100000)]
		[TestCase(200000)]
		[TestCase(500000)]
		public void TestSet(int iter)
		{
			var target = new T();
			var stopwatch = new Stopwatch();

			// Warm up
			TestSetter(_reflectionSetter, target, 100);
			TestSetter(_ilSetter, target, 100);

			stopwatch.Restart();
			TestSetter(_reflectionSetter, target, iter);
			stopwatch.Stop();
			Console.WriteLine($"Reflection setter total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestSetter(_ilSetter, target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IL setter total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");
		}

		private static void TestGetter(IGetter getter, object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				var val = getter.Get(target);
			}
		}

		private void TestSetter(ISetter setter, object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				setter.Set(target, GetValue());
			}
		}
	}
}
