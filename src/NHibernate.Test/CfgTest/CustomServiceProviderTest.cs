using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class CustomServiceProviderTest
	{
		private class MyServiceProvider : IServiceProvider
		{
			public object GetService(System.Type serviceType)
			{
				throw new NotImplementedException();
			}
		}
		private class InvalidServiceProvider
		{
		}
		private class InvalidNoCtorServiceProvider : MyServiceProvider
		{
			public InvalidNoCtorServiceProvider(string pizza) {}
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildServiceProvider()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(MyServiceProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildServiceProvider(properties), Throws.Nothing);
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildInstanceOfConfiguredServiceProvider()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyServiceProvider, typeof(MyServiceProvider).AssemblyQualifiedName } };
			Assert.That(Environment.BuildServiceProvider(properties), Is.InstanceOf<MyServiceProvider>());
		}

		[Test]
		public void WhenInvalidThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyServiceProvider, typeof(InvalidServiceProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildServiceProvider(properties), Throws.TypeOf<HibernateServiceProviderException>());
		}

		[Test]
		public void WhenNoDefaultCtorThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyServiceProvider, typeof(InvalidNoCtorServiceProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildServiceProvider(properties), Throws.TypeOf<HibernateServiceProviderException>()
																				   .And.InnerException.Message.Contains("constructor was not found"));
		}
	}
}
