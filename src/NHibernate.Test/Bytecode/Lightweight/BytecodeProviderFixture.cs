using System;
using NHibernate.Bytecode;
using NHibernate.Bytecode.Lightweight;
using NUnit.Framework;
using SharpTestsEx;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.Bytecode.Lightweight
{
	[TestFixture]
	public class BytecodeProviderFixture
	{
		[Test]
		public void NotConfiguredProxyFactoryFactory()
		{
			var bcp = new BytecodeProviderImpl();
			IProxyFactoryFactory p = bcp.ProxyFactoryFactory;
			p.Should().Be.InstanceOf<DefaultProxyFactoryFactory>();
		}

		[Test]
		public void UnableToLoadProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory("whatever");
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message, Is.StringStarting("Unable to load type"));
				Assert.That(e.Message, Is.StringContaining("Possible causes"));
				Assert.That(e.Message, Is.StringContaining("Confirm that your deployment folder contains"));
			}
		}

		[Test]
		public void DoesNotImplementProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory(GetType().AssemblyQualifiedName);
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message,
										Is.EqualTo(GetType().FullName + " does not implement " + typeof(IProxyFactoryFactory).FullName));
			}
		}

		[Test]
		public void CantCreateProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory(typeof(WrongProxyFactoryFactory).AssemblyQualifiedName);
				IProxyFactoryFactory p = bcp.ProxyFactoryFactory;
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message,Is.StringStarting("Failed to create an instance of"));
			}
		}

		[Test]
		public void NotConfiguredCollectionTypeFactory()
		{
			// our BytecodeProvider should ever have a CollectionTypeFactory
			var bcp = new BytecodeProviderImpl();
			Assert.That(bcp.CollectionTypeFactory, Is.Not.Null);
		}

		[Test]
		public void SetCollectionTypeFactoryClassByName()
		{
			string nullName = null;
			var bcp = new BytecodeProviderImpl();

			Assert.Throws<ArgumentNullException>(() => bcp.SetCollectionTypeFactoryClass(nullName));
			Assert.Throws<ArgumentNullException>(() => bcp.SetCollectionTypeFactoryClass(string.Empty));
			Assert.Throws<TypeLoadException>(() => bcp.SetCollectionTypeFactoryClass("whatever"));
		}

		[Test]
		public void SetCollectionTypeFactoryClassByType()
		{
			System.Type nullType = null;
			var bcp = new BytecodeProviderImpl();
			Assert.Throws<ArgumentNullException>(() => bcp.SetCollectionTypeFactoryClass(nullType));
			Assert.Throws<HibernateByteCodeException>(() => bcp.SetCollectionTypeFactoryClass(GetType()), "should allow only ICollectionTypeFactory type");
		}

		private class NoDefaultCtor: Type.DefaultCollectionTypeFactory
		{
			public NoDefaultCtor(int something) {}
		}

		[Test]
		public void InvalidCollectionTypeFactoryCtor()
		{
			ICollectionTypeFactory ctf;
			var bcp = new BytecodeProviderImpl();
			bcp.SetCollectionTypeFactoryClass(typeof (NoDefaultCtor));
			Assert.Throws<HibernateByteCodeException>(() => ctf = bcp.CollectionTypeFactory);
		}

		[Test]
		public void CollectionTypeFactoryCantChangeAfterUsage()
		{
			ICollectionTypeFactory ctf;
			var bcp = new BytecodeProviderImpl();
			ctf = bcp.CollectionTypeFactory; // initialize the instance
			// try to set it
			Assert.Throws<InvalidOperationException>(() => bcp.SetCollectionTypeFactoryClass(typeof(CustomCollectionTypeFactory)));
		}

		private class CustomCollectionTypeFactory : Type.DefaultCollectionTypeFactory
		{
		}

		[Test]
		[Explicit("The BytecodeProvider is static and can't be different in the same application.")]
		public void AllowCustomCollectionTypeFactoryBeforeBuildFirstMapping()
		{
			// Allow set of CustomCollectionTypeFactory class after configure BUT before add the first mapping.
			// for real we need CustomCollectionTypeFactory before BuildSessionFactory but for possible future
			// "mapping-sources" is better to limitate the moment of injectability.
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.CollectionTypeFactoryClass, typeof(CustomCollectionTypeFactory).AssemblyQualifiedName);
			Dialect.Dialect dialect = Dialect.Dialect.GetDialect(cfg.Properties);
			cfg.CreateMappings(dialect);
			Assert.That(Environment.BytecodeProvider.CollectionTypeFactory, Is.TypeOf<CustomCollectionTypeFactory>());
		}

		[Test]
		[Explicit("The BytecodeProvider is static and can't be different in the same application.")]
		public void WorkAddingMappings()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.CollectionTypeFactoryClass, typeof(CustomCollectionTypeFactory).AssemblyQualifiedName);
			cfg.AddResource("NHibernate.Test.Bytecode.Lightweight.ProductLine.hbm.xml", GetType().Assembly);
			Assert.That(Environment.BytecodeProvider.CollectionTypeFactory, Is.TypeOf<CustomCollectionTypeFactory>());
		}

		[Test]
		[Explicit("The BytecodeProvider is static and can't be different in the same application.")]
		public void ShouldNotThrownAnExceptionWithTheSameTypeOfCollectionTypeFactory()
		{
			ICollectionTypeFactory ctf;
			var bcp = new BytecodeProviderImpl();
			ctf = bcp.CollectionTypeFactory; // initialize the instance
			Assert.DoesNotThrow(() => bcp.SetCollectionTypeFactoryClass(typeof (Type.DefaultCollectionTypeFactory)));
		}
	}
}