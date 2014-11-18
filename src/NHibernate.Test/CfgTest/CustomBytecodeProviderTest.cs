using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NHibernate.Properties;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CfgTest
{
	public class CustomBytecodeProviderTest
	{
		private class MyByteCodeProvider : AbstractBytecodeProvider
		{
			public override IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
			{
				throw new NotImplementedException();
			}
		}
		private class InvalidByteCodeProvider
		{
		}
		private class InvalidNoCtorByteCodeProvider : AbstractBytecodeProvider
		{
			public InvalidNoCtorByteCodeProvider(string pizza) {}

			public override IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildBytecodeProvider()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(MyByteCodeProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildBytecodeProvider(properties), Throws.Nothing);
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildInstanceOfConfiguredBytecodeProvider()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(MyByteCodeProvider).AssemblyQualifiedName } };
			Assert.That(Environment.BuildBytecodeProvider(properties), Is.InstanceOf<MyByteCodeProvider>());
		}

		[Test]
		public void WhenInvalidThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(InvalidByteCodeProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildBytecodeProvider(properties), Throws.TypeOf<HibernateByteCodeException>());
		}

		[Test]
		public void WhenNoDefaultCtorThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(InvalidNoCtorByteCodeProvider).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildBytecodeProvider(properties), Throws.TypeOf<HibernateByteCodeException>()
																				   .And.InnerException.Message.ContainsSubstring("constructor was not found"));
		}
	}
}