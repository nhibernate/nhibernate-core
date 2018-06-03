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
		internal interface IInternal
		{
			int Id { get; }
		}

		[Serializable]
		public class InternalInterfaceTestClass : IInternal
		{
			public virtual int Id { get; set; }
		}

		public interface IPublic
		{
			int Id { get; }
		}

		[Serializable]
		public class PublicInterfaceTestClass : IPublic
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

		[Serializable]
		public class CustomExplicitSerializationClass : ISerializable
		{
			public virtual int Id { get; set; }

			public CustomExplicitSerializationClass()
			{
			}

			protected CustomExplicitSerializationClass(SerializationInfo info, StreamingContext context)
			{
				Id = info.GetInt32(nameof(Id));
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				GetObjectData(info, context);
			}

			protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue(nameof(Id), Id);
			}
		}

		[Test]
		public void VerifyProxyForClassWithInternalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(InternalInterfaceTestClass).FullName, typeof(InternalInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyProxyForClassWithAdditionalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(PublicInterfaceTestClass).FullName,
				typeof(PublicInterfaceTestClass),
				// By way of the "proxy" attribute on the "class" mapping, an interface to use for the
				// lazy entity load proxy instead of the persistentClass can be specified. This is "translated" into
				// having an additional interface in the interface list, instead of just having INHibernateProxy.
				// (Quite a loosy semantic...)
				new HashSet<System.Type> {typeof(INHibernateProxy), typeof(IPublic)},
				null, null, null);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.Not.InstanceOf<PublicInterfaceTestClass>());
#if NETFX
				});
#endif
		}

		[Test]
		public void CanSerializeFieldInterceptorProxy()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(PublicInterfaceTestClass).FullName, typeof(PublicInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = (PublicInterfaceTestClass) factory.GetFieldInterceptionProxy(new PublicInterfaceTestClass());
			proxy.Id = 1;

			var serializer = GetFormatter();
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				proxy = (PublicInterfaceTestClass) serializer.Deserialize(memoryStream);
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

		[Test]
		public void CanSerializeFieldInterceptorProxyWithExplicitISerializableEntity()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(CustomExplicitSerializationClass).FullName, typeof(CustomExplicitSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = (CustomExplicitSerializationClass) factory.GetFieldInterceptionProxy(new CustomExplicitSerializationClass());
			proxy.Id = 2;

			var serializer = GetFormatter();
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				proxy = (CustomExplicitSerializationClass) serializer.Deserialize(memoryStream);
				Assert.That(proxy.Id, Is.EqualTo(2));
			}
		}

		[Test]
		public void VerifyFieldInterceptorProxy()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(InternalInterfaceTestClass).FullName, typeof(InternalInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy(new InternalInterfaceTestClass());
					Assert.That(fieldProxy, Is.InstanceOf<InternalInterfaceTestClass>());
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyFieldInterceptorProxyWithISerializableEntity()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(CustomSerializationClass).FullName, typeof(CustomSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy(new CustomSerializationClass());
					Assert.That(fieldProxy, Is.InstanceOf<CustomSerializationClass>());
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyFieldInterceptorProxyWithAdditionalInterface()
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
				// to an instance of the persistentClass, and so cannot implement interface methods if it does not
				// inherit the persitentClass.
				new HashSet<System.Type> {typeof(INHibernateProxy), typeof(IPublic)},
				null, null, null);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy(new PublicInterfaceTestClass());
					Assert.That(fieldProxy, Is.InstanceOf<PublicInterfaceTestClass>());
#if NETFX
				});
#endif
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

#if NETFX
		private static void VerifyGeneratedAssembly(System.Action assemblyGenerator)
		{
			var proxyBuilderHelperType = typeof(StaticProxyFactory).Assembly.GetType("NHibernate.Proxy.ProxyBuilderHelper", true);
			var enableSave = proxyBuilderHelperType.GetMethod("EnableDynamicAssemblySaving", BindingFlags.NonPublic | BindingFlags.Static);
			Assert.That(enableSave, Is.Not.Null, "Failed to find method EnableDynamicAssemblySaving");

			const string assemblyName = "VerifyFieldInterceptorProxy.dll";
			var assemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, assemblyName);
			enableSave.Invoke(null, new object[] { true, assemblyPath });
			try
			{
				assemblyGenerator();
			}
			finally
			{
				enableSave.Invoke(null, new object[] { false, null });
			}

			new PeVerifier(assemblyName).AssertIsValid();
		}
#endif
	}
}
