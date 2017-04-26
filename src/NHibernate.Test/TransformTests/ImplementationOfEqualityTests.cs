using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests
{
	public class ImplementationOfEqualityTests
	{
		private readonly IEnumerable<System.Type> transformerTypes =
			typeof (IResultTransformer).Assembly.GetTypes()
									   .Where(t => typeof (IResultTransformer).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
									   .ToList();

		[Test]
		public void AllEmbeddedTransformersOverridesEqualsAndGetHashCode()
		{
			foreach (var transformerType in transformerTypes)
			{
				var declaredMethods = transformerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				Assert.That(declaredMethods.Select(x => x.Name), Contains.Item("Equals"), "The type " + transformerType + " does not implement Equals");
				Assert.That(declaredMethods.Select(x => x.Name), Contains.Item("GetHashCode"), "The type " + transformerType + " does not implement GetHashCode");
			}
		}

		[Test]
		public void AllEmbeddedTransformersWithDefaultCtorHasEqualityWorkingAsSingleton()
		{
			foreach (var transformerType in transformerTypes)
			{
				IResultTransformer transformer1;
				try
				{
					transformer1 = (IResultTransformer)Activator.CreateInstance(transformerType);
				}
				catch (Exception e)
				{
					// The transformer does not have default ctor
					Console.WriteLine(transformerType);
					Console.WriteLine(e);
					continue;
				}
				var transformer2= (IResultTransformer)Activator.CreateInstance(transformerType);
				Assert.That(transformer1, Is.EqualTo(transformer2));
				Assert.That(transformer1.GetHashCode(), Is.EqualTo(transformer2.GetHashCode()));
			}
		}

		[Test]
		public void AliasToBeanResultTransformer_ShouldHaveEqualityBasedOnCtorParameter()
		{
			var transformer1 = new AliasToBeanResultTransformer(typeof(object));
			var transformer2 = new AliasToBeanResultTransformer(typeof(object));
			Assert.That(transformer1, Is.EqualTo(transformer2));
			Assert.That(transformer1.GetHashCode(), Is.EqualTo(transformer2.GetHashCode()));

			var transformer3 = new AliasToBeanResultTransformer(typeof(int));
			Assert.That(transformer1, Is.Not.EqualTo(transformer3));
			Assert.That(transformer1.GetHashCode(), Is.Not.EqualTo(transformer3.GetHashCode()));
		}

		[Test]
		public void AliasToBeanConstructorResultTransformer_ShouldHaveEqualityBasedOnCtorParameter()
		{
			var emptyCtor = new System.Type[0];
			var transformer1 = new AliasToBeanConstructorResultTransformer(typeof(object).GetConstructor(emptyCtor));
			var transformer2 = new AliasToBeanConstructorResultTransformer(typeof(object).GetConstructor(emptyCtor));
			Assert.That(transformer1, Is.EqualTo(transformer2));
			Assert.That(transformer1.GetHashCode(), Is.EqualTo(transformer2.GetHashCode()));

			var transformer3 = new AliasToBeanConstructorResultTransformer(typeof(ImplementationOfEqualityTests).GetConstructor(emptyCtor));
			Assert.That(transformer1, Is.Not.EqualTo(transformer3));
			Assert.That(transformer1.GetHashCode(), Is.Not.EqualTo(transformer3.GetHashCode()));
		}

		[Test]
		public void LinqResultTransformer_ShouldHaveEqualityBasedOnCtorParameter()
		{
			Func<object[], object> d1 = x => new object();
			Func<IEnumerable<object>, IEnumerable<object>> d2 = x => x;
			var transformer1 = new ResultTransformer(d1, d2);
			var transformer2 = new ResultTransformer(d1, d2);
			Assert.That(transformer1, Is.EqualTo(transformer2));
			Assert.That(transformer1.GetHashCode(), Is.EqualTo(transformer2.GetHashCode()));

			Func<IEnumerable<object>, IEnumerable<int>> d3 = x => new [] { 1, 2, 3 };
			var transformer3 = new ResultTransformer(d1, d3);
			Assert.That(transformer1, Is.Not.EqualTo(transformer3));
			Assert.That(transformer1.GetHashCode(), Is.Not.EqualTo(transformer3.GetHashCode()));
		}
	}
}