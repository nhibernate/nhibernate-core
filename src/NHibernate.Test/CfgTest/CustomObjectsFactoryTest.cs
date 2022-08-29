using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class CustomObjectsFactoryTest
	{
		private class MyObjectsFactory : IObjectsFactory
		{
			public object CreateInstance(System.Type type)
			{
				throw new NotImplementedException();
			}

			public object CreateInstance(System.Type type, bool nonPublic)
			{
				throw new NotImplementedException();
			}

			public object CreateInstance(System.Type type, params object[] ctorArgs)
			{
				throw new NotImplementedException();
			}
		}
		private class InvalidObjectsFactory
		{
		}
		private class InvalidNoCtorObjectsFactory : MyObjectsFactory
		{
			public InvalidNoCtorObjectsFactory(string pizza) { }
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildObjectsFactory()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyBytecodeProvider, typeof(MyObjectsFactory).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildObjectsFactory(properties), Throws.Nothing);
		}

		[Test]
		public void WhenNoShortCutUsedThenCanBuildInstanceOfConfiguredObjectsFactory()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyObjectsFactory, typeof(MyObjectsFactory).AssemblyQualifiedName } };
			Assert.That(Environment.BuildObjectsFactory(properties), Is.InstanceOf<MyObjectsFactory>());
		}

		[Test]
		public void WhenInvalidThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyObjectsFactory, typeof(InvalidObjectsFactory).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildObjectsFactory(properties), Throws.TypeOf<HibernateObjectsFactoryException>());
		}

		[Test]
		public void WhenNoDefaultCtorThenThrow()
		{
			var properties = new Dictionary<string, string> { { Environment.PropertyObjectsFactory, typeof(InvalidNoCtorObjectsFactory).AssemblyQualifiedName } };
			Assert.That(() => Environment.BuildObjectsFactory(properties), Throws.TypeOf<HibernateObjectsFactoryException>()
																				   .And.InnerException.Message.Contains("constructor was not found"));
		}
	}
}
