using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq.Functions;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Linq
{
	public class LinqToHqlGeneratorsRegistryFactoryTest
	{
		[Test]
		public void WhenNotDefinedThenReturnDefaultRegistry()
		{
			var registry = LinqToHqlGeneratorsRegistryFactory.CreateGeneratorsRegistry(new Dictionary<string, string>());
			Assert.That(registry, Is.Not.Null);
			Assert.That(registry, Is.TypeOf<DefaultLinqToHqlGeneratorsRegistry>());
		}

		[Test]
		public void WhenDefinedThenReturnCustomtRegistry()
		{
			var properties = new Dictionary<string, string> { { Environment.LinqToHqlGeneratorsRegistry, typeof(MyLinqToHqlGeneratorsRegistry).AssemblyQualifiedName } };
			var registry = LinqToHqlGeneratorsRegistryFactory.CreateGeneratorsRegistry(properties);
			Assert.That(registry, Is.Not.Null);
			Assert.That(registry, Is.TypeOf<MyLinqToHqlGeneratorsRegistry>());
		}

		private class MyLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
		{
			public bool TryGetGenerator(MethodInfo method, out IHqlGeneratorForMethod generator)
			{
				throw new NotImplementedException();
			}

			public bool TryGetGenerator(MemberInfo property, out IHqlGeneratorForProperty generator)
			{
				throw new NotImplementedException();
			}

			public void RegisterGenerator(MethodInfo method, IHqlGeneratorForMethod generator)
			{
				throw new NotImplementedException();
			}

			public void RegisterGenerator(MemberInfo property, IHqlGeneratorForProperty generator)
			{
				throw new NotImplementedException();
			}

			public void RegisterGenerator(IRuntimeMethodHqlGenerator generator)
			{
				throw new NotImplementedException();
			}
		}
	}
}