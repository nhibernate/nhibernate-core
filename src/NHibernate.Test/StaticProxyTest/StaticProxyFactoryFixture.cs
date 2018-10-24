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
			int Id { get; set; }
			string Name { get; set; }
		}

		[Serializable]
		public class PublicInterfaceTestClass : IPublic
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }

			public PublicInterfaceTestClass()
			{
				// Check access to properties from the default constructor do not fail once proxified
				Assert.That(Id, Is.Zero);
				Assert.That(Name, Is.Null);
				Id = -1;
				Name = string.Empty;
			}
		}

		[Serializable]
		public class PublicExplicitInterfaceTestClass : IPublic
		{
			int IPublic.Id { get; set; }
			string IPublic.Name { get; set; }

			public PublicExplicitInterfaceTestClass()
			{
				// Check access to properties from the default constructor do not fail once proxified
				IPublic pub = this;
				Assert.That(pub.Id, Is.Zero);
				Assert.That(pub.Name, Is.Null);
				pub.Id = -1;
				pub.Name = string.Empty;
			}
		}

		[Serializable]
		public abstract class AbstractTestClass : IPublic
		{
			protected AbstractTestClass()
			{
				Assert.That(Id, Is.Zero);
				Assert.That(Name, Is.Null);
				Id = -1;
				Name = "Unknown";
			}

			public abstract int Id { get; set; }
			
			public abstract string Name { get; set; } 
		}

		[Serializable]
		public class SimpleTestClass
		{
			public virtual int Id { get; set; }
		}

		[Serializable]
		public class RefOutTestClass
		{
			public virtual int Id { get; set; }

			public virtual void Method1(out int x)
			{
				x = 3;
			}

			public virtual void Method2(ref int x)
			{
				x++;
			}
		
			public virtual void Method3(out int? y)
			{
				y = 4;
			}

			public virtual void Method4(ref int? y)
			{
				y++;
			}

			public virtual void Method(ref Dictionary<string, string> dictionary)
			{
				dictionary = dictionary == null
					? new Dictionary<string, string>()
					: new Dictionary<string, string>(dictionary);
			}
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
				// (Quite a loose semantic...)
				new HashSet<System.Type> { typeof(INHibernateProxy), typeof(IPublic) },
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
		public void VerifyProxyForClassWithInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(PublicInterfaceTestClass).FullName,
				typeof(PublicInterfaceTestClass),
				new HashSet<System.Type> {typeof(INHibernateProxy)},
				null, null, null);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<PublicInterfaceTestClass>());

					// Check interface and implicit implementations do both call the delegated state
					var state = new PublicInterfaceTestClass { Id = 5, Name = "State" };
					proxy.HibernateLazyInitializer.SetImplementation(state);
					var pub = (IPublic) proxy;
					var ent = (PublicInterfaceTestClass) proxy;
					Assert.That(pub.Id, Is.EqualTo(5), "IPublic.Id");
					Assert.That(ent.Id, Is.EqualTo(5), "entity.Id");
					Assert.That(pub.Name, Is.EqualTo("State"), "IPublic.Name");
					Assert.That(ent.Name, Is.EqualTo("State"), "entity.Name");
					ent.Id = 10;
					pub.Name = "Test";
					Assert.That(pub.Id, Is.EqualTo(10), "IPublic.Id");
					Assert.That(state.Id, Is.EqualTo(10), "state.Id");
					Assert.That(ent.Name, Is.EqualTo("Test"), "entity.Name");
					Assert.That(state.Name, Is.EqualTo("Test"), "state.Name");
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyProxyForClassWithExplicitInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(PublicExplicitInterfaceTestClass).FullName,
				typeof(PublicExplicitInterfaceTestClass),
				new HashSet<System.Type> {typeof(INHibernateProxy)},
				null, null, null);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<PublicExplicitInterfaceTestClass>());
					
					// Check interface and implicit implementations do both call the delegated state
					IPublic state = new PublicExplicitInterfaceTestClass();
					state.Id = 5;
					state.Name = "State";
					proxy.HibernateLazyInitializer.SetImplementation(state);
					var entity = (IPublic) proxy;
					Assert.That(entity.Id, Is.EqualTo(5), "Id");
					Assert.That(entity.Name, Is.EqualTo("State"), "Name");
					
					entity.Id = 10;
					entity.Name = "Test";
					Assert.That(entity.Id, Is.EqualTo(10), "entity.Id");
					Assert.That(state.Id, Is.EqualTo(10), "state.Id");
					Assert.That(entity.Name, Is.EqualTo("Test"), "entity.Name");
					Assert.That(state.Name, Is.EqualTo("Test"), "state.Name");
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyProxyForRefOutClass()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(RefOutTestClass).FullName,
				typeof(RefOutTestClass),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null,
				null,
				null);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);

					var state = new RefOutTestClass();
					proxy.HibernateLazyInitializer.SetImplementation(state);

					var entity = (RefOutTestClass) proxy;
					entity.Method1(out var x);
					Assert.That(x, Is.EqualTo(3));

					entity.Method2(ref x);
					Assert.That(x, Is.EqualTo(4));

					entity.Method3(out var y);
					Assert.That(y, Is.EqualTo(4));

					entity.Method4(ref y);
					Assert.That(y, Is.EqualTo(5));

					var dictionary = new Dictionary<string, string>();
					var param = dictionary;
					entity.Method(ref param);
					Assert.That(param, Is.Not.SameAs(dictionary));
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyProxyForAbstractClass()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(AbstractTestClass).FullName,
				typeof(AbstractTestClass),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null, null, null);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<AbstractTestClass>());
#if NETFX
				});
#endif
		}

		[Test]
		public void InitializedProxyStaysInitializedAfterDeserialization()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(2, null);
			Assert.That(proxy, Is.Not.Null, "proxy");
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.False, "proxy already initialized after creation");
			Assert.That(proxy.HibernateLazyInitializer, Is.Not.Null, "HibernateLazyInitializer");

			var impl = new SimpleTestClass { Id = 2 };
			proxy.HibernateLazyInitializer.SetImplementation(impl);
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.True, "proxy not initialized after setting implementation");

			var serializer = GetFormatter();
			object deserialized;
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				deserialized = serializer.Deserialize(memoryStream);
			}
			Assert.That(deserialized, Is.Not.Null, "deserialized");
			Assert.That(deserialized, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(deserialized), Is.True, "proxy no more initialized after deserialization");
			Assert.That(deserialized, Is.InstanceOf<SimpleTestClass>());
			Assert.That(((SimpleTestClass) deserialized).Id, Is.EqualTo(2));
		}

		[Test]
		public void NonInitializedProxyStaysNonInitializedAfterSerialization()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(2, null);
			Assert.That(proxy, Is.Not.Null, "proxy");
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.False, "proxy already initialized after creation");

			var serializer = GetFormatter();
			object deserialized;
			using (var memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, proxy);
				Assert.That(NHibernateUtil.IsInitialized(proxy), Is.False, "proxy initialized after serialization");
				memoryStream.Seek(0L, SeekOrigin.Begin);
				deserialized = serializer.Deserialize(memoryStream);
			}
			Assert.That(deserialized, Is.Not.Null, "deserialized");
			Assert.That(deserialized, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(deserialized), Is.False, "proxy initialized after deserialization");
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
				// (Quite a loose semantic...)
				// The field interceptor proxy ignores this setting, as it does not delegate its implementation
				// to an instance of the persistentClass, and so cannot implement interface methods if it does not
				// inherit the persistentClass.
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
