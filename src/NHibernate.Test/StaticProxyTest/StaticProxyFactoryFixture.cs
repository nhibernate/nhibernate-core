using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using NHibernate.Proxy;
using NUnit.Framework;

#if NETFX
using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Test.ProxyTest;
#else
using System.Globalization;
using NHibernate.Test.Legacy;
using NHibernate.Util;
#endif

namespace NHibernate.Test.StaticProxyTest
{
	public class StaticProxyFactoryFixture
	{
		internal interface ISomething
		{
			int Id { get; }
		}

		[Serializable]
		public class TestClass : ISomething
		{
			public virtual int Id { get; set; }
		}

		[Serializable]
		public class CustomSerializationClass : ISerializable
		{
			public virtual int Id { get; set; }

			public CustomSerializationClass()
			{
			}

			protected CustomSerializationClass(SerializationInfo info, StreamingContext context)
			{
				Id = info.GetInt32(nameof(Id));
			}

			[SecurityCritical]
			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue(nameof(Id), Id);
			}
		}

		[Test]
		public void CanCreateProxyForClassWithInternalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(TestClass).FullName, typeof(TestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(1, null);
			Assert.That(proxy, Is.Not.Null);
		}

		[Test]
		public void CanSerializeFieldInterceptorProxy()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(TestClass).FullName, typeof(TestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = (TestClass) factory.GetFieldInterceptionProxy(new TestClass());
			proxy.Id = 1;

			var serializer = GetFormatter();
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				proxy = (TestClass) serializer.Deserialize(memoryStream);
				Assert.That(proxy.Id, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanSerializeFieldInterceptorProxyWithISerializableEntity()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(CustomSerializationClass).FullName, typeof(CustomSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = (CustomSerializationClass) factory.GetFieldInterceptionProxy(new CustomSerializationClass());
			proxy.Id = 2;

			var serializer = GetFormatter();
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				proxy = (CustomSerializationClass) serializer.Deserialize(memoryStream);
				Assert.That(proxy.Id, Is.EqualTo(2));
			}
		}

#if NETFX
		[Test]
		public void VerifyFieldInterceptorProxy()
		{
			var proxyBuilderType = typeof(StaticProxyFactory).Assembly.GetType("NHibernate.Proxy.FieldInterceptorProxyBuilder", true);
			var proxyBuilder = proxyBuilderType.GetMethod("CreateProxyType");
			Assert.That(proxyBuilder, Is.Not.Null, "Failed to find method CreateProxyType");
			var proxyBuilderAssemblyBuilder = proxyBuilderType.GetField("ProxyAssemblyBuilder", BindingFlags.NonPublic | BindingFlags.Static);
			Assert.That(proxyBuilderAssemblyBuilder, Is.Not.Null, "Failed to find assembly builder field");

			const string assemblyName = "VerifyFieldInterceptorProxy";
			var assemblyBuilder = new SavingProxyAssemblyBuilder(assemblyName);

			var backupAssemblyBuilder = proxyBuilderAssemblyBuilder.GetValue(null);
			proxyBuilderAssemblyBuilder.SetValue(null, assemblyBuilder);
			try
			{
				proxyBuilder.Invoke(null, new object[] { typeof(TestClass) });
			}
			finally
			{
				proxyBuilderAssemblyBuilder.SetValue(null, backupAssemblyBuilder);
			}

			new PeVerifier($"{assemblyName}.dll").AssertIsValid();
		}

		public class SavingProxyAssemblyBuilder : IProxyAssemblyBuilder
		{
			private readonly string _assemblyName;

			public SavingProxyAssemblyBuilder(string assemblyName)
			{
				_assemblyName = assemblyName;
			}

			public AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name)
			{
				return appDomain.DefineDynamicAssembly(
					new AssemblyName(_assemblyName),
					AssemblyBuilderAccess.RunAndSave,
					TestContext.CurrentContext.TestDirectory);
			}

			public ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName)
			{
				return assemblyBuilder.DefineDynamicModule(moduleName, $"{_assemblyName}.mod", true);
			}

			public void Save(AssemblyBuilder assemblyBuilder)
			{
				assemblyBuilder.Save($"{_assemblyName}.dll");
			}
		}
#endif

		public interface IPublicTest
		{
			int Id { get; }
		}

		public class PublicInterfaceTestClass : IPublicTest
		{
			public virtual int Id { get; set; }
		}

		[Test]
		public void CanGenerateValidFieldInterceptorProxyWithAdditionalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(PublicInterfaceTestClass).FullName,
				typeof(PublicInterfaceTestClass),
				// By way of the "proxy" attribute on the "class" mapping, an interface to use for the
				// lazy entity load proxy instead of the persistentClass can be specified. This is "translated" into
				// having an additional interface in the interface list, instead of just having INHibernateProxy.
				// (Quite a loosy semantic...)
				// The field interceptor proxy ignores this setting, as it does not delegate its implementation
				// to an instance of the persistentClass, and so cannot implement interface methods.
				new HashSet<System.Type> {typeof(INHibernateProxy), typeof(IPublicTest)},
				null, null, null);
			var fieldProxy = factory.GetFieldInterceptionProxy(null);
			Assert.That(fieldProxy, Is.InstanceOf<PublicInterfaceTestClass>());
		}

		private static BinaryFormatter GetFormatter()
		{
#if NETFX
			return new BinaryFormatter();
#else
			var selector = new SurrogateSelector();
			selector.AddSurrogate(
				typeof(CultureInfo),
				new StreamingContext(StreamingContextStates.All),
				new CultureInfoSerializationSurrogate());
			selector.ChainSelector(new SerializationHelper.SurrogateSelector());
			return new BinaryFormatter
			{
				SurrogateSelector = selector
			};
#endif
		}
	}
}
