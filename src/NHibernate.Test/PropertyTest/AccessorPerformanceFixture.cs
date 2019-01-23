using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NHibernate.Bytecode;
using NHibernate.Bytecode.Lightweight;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	public abstract class AccessorPerformanceFixture<T> where T : new()
	{
		private IPropertyAccessor _accessor;
		private IAccessOptimizer _optimizer;
		private ISetter[] _setters;
		private IGetter[] _getters;

		protected abstract string AccessorType { get; }

		protected abstract List<string> PropertyNames { get; }

		protected abstract object[] GetValues();

		[SetUp]
		public void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor(AccessorType);

			_getters = new IGetter[PropertyNames.Count];
			_setters = new ISetter[PropertyNames.Count];
			var type = typeof(T);

			for (var i = 0; i < PropertyNames.Count; i++)
			{
				_getters[i] = _accessor.GetGetter(type, PropertyNames[i]);
				_setters[i] = _accessor.GetSetter(type, PropertyNames[i]);
			}

			_optimizer = new ReflectionOptimizer(type, _getters, _setters, null, null).AccessOptimizer;
		}

		[TestCase(50000)]
		[TestCase(100000)]
		[TestCase(200000)]
		[TestCase(500000)]
		public void TestGetter(int iter)
		{
			var target = new T();
			var stopwatch = new Stopwatch();

			// Warm up
			TestGetter(target, 100);
			TestOptimizedGetter(target, 100);
			TestOptimizedMultiGetter(target, 100);

			stopwatch.Restart();
			TestGetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IGetter.Get total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestOptimizedGetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IAccessOptimizer.GetPropertyValue total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestOptimizedMultiGetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IAccessOptimizer.GetPropertyValues total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");
		}

		[TestCase(50000)]
		[TestCase(100000)]
		[TestCase(200000)]
		[TestCase(500000)]
		public void TestSetter(int iter)
		{
			var target = new T();
			var stopwatch = new Stopwatch();

			// Warm up
			TestSetter(target, 100);
			TestOptimizedSetter(target, 100);
			TestOptimizedMultiSetter(target, 100);

			stopwatch.Restart();
			TestSetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"ISetter.Set total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestOptimizedSetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IAccessOptimizer.SetPropertyValue total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");

			stopwatch.Restart();
			TestOptimizedMultiSetter(target, iter);
			stopwatch.Stop();
			Console.WriteLine($"IAccessOptimizer.SetPropertyValues total time for {iter} iterations: {stopwatch.ElapsedMilliseconds}ms");
		}

		private void TestGetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				for (var j = 0; j < _getters.Length; j++)
				{
					var val = _getters[j].Get(target);
				}
			}
		}

		private void TestOptimizedGetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				for (var j = 0; j < _getters.Length; j++)
				{
					var val = _optimizer.GetPropertyValue(target, j);
				}
			}
		}

		private void TestOptimizedMultiGetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				var val = _optimizer.GetPropertyValues(target);
			}
		}

		private void TestSetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				var values = GetValues();
				for (var j = 0; j < _setters.Length; j++)
				{
					_setters[j].Set(target, values[j]);
				}
			}
		}

		private void TestOptimizedSetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				var values = GetValues();
				for (var j = 0; j < _setters.Length; j++)
				{
					_optimizer.SetPropertyValue(target, j, values[j]);
				}
			}
		}

		private void TestOptimizedMultiSetter(object target, int iter)
		{
			for (var i = 0; i < iter; i++)
			{
				_optimizer.SetPropertyValues(target, GetValues());
			}
		}
	}
}
