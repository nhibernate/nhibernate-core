using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NHibernate.Properties;
using NUnit.Framework;
using SharpTestsEx;
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
			Executing.This(() => Environment.BuildBytecodeProvider(properties)).Should().NotThrow();
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildInstanceOfConfiguredBytecodeProvider()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(MyByteCodeProvider).AssemblyQualifiedName } };
			Environment.BuildBytecodeProvider(properties).Should().Be.InstanceOf<MyByteCodeProvider>();
		}

		[Test]
		public void WhenInvalidThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(InvalidByteCodeProvider).AssemblyQualifiedName } };
			Executing.This(() => Environment.BuildBytecodeProvider(properties)).Should().Throw<HibernateByteCodeException>();
		}

		[Test]
		public void WhenNoDefaultCtorThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(InvalidNoCtorByteCodeProvider).AssemblyQualifiedName } };
			Executing.This(() => Environment.BuildBytecodeProvider(properties)).Should().Throw<HibernateByteCodeException>()
				.And.Exception.InnerException.Message.Should().Contain("constructor was not found");
		}
	}
}