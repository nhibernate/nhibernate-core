using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq.Functions;
using NUnit.Framework;
using SharpTestsEx;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Linq
{
	public class LinqToHqlGeneratorsRegistryFactoryTest
	{
		[Test]
		public void WhenNotDefinedThenReturnDefaultRegistry()
		{
			var registry = LinqToHqlGeneratorsRegistryFactory.CreateGeneratorsRegistry(new Dictionary<string, string>());
			registry.Should().Not.Be.Null();
			registry.Should().Be.OfType<DefaultLinqToHqlGeneratorsRegistry>();
		}

		[Test]
		public void WhenDefinedThenReturnCustomtRegistry()
		{
			var properties = new Dictionary<string, string> { { Environment.LinqToHqlGeneratorsRegistry, typeof(MyLinqToHqlGeneratorsRegistry).AssemblyQualifiedName } };
			var registry = LinqToHqlGeneratorsRegistryFactory.CreateGeneratorsRegistry(properties);
			registry.Should().Not.Be.Null();
			registry.Should().Be.OfType<MyLinqToHqlGeneratorsRegistry>();
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