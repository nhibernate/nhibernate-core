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
		
		public interface IWithEqualsAndGetHashCode
		{
			bool Equals(object that);
			int GetHashCode();
		}

		[Serializable]
		public class InternalInterfaceTestClass : IInternal
		{
			public virtual int Id { get; set; }
		}

		[Serializable]
		internal class InternalTestClass
		{
			int Id { get; set; }
			string Name { get; set; }
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
				Id = -1;
				Assert.That(Id, Is.EqualTo(-1));
				Name = "Unknown";
				Assert.That(Name, Is.EqualTo("Unknown"));
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
				pub.Id = -1;
				Assert.That(pub.Id, Is.EqualTo(-1));
				pub.Name = "Unknown";
				Assert.That(pub.Name, Is.EqualTo("Unknown"));
			}
		}

		[Serializable]
		public class PublicExplicitInterfaceWithSameMembersTestClass : IPublic
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }

			int IPublic.Id { get; set; }
			string IPublic.Name { get; set; }

			public PublicExplicitInterfaceWithSameMembersTestClass()
			{
				// Check access to properties from the default constructor do not fail once proxified
				Id = -1;
				Assert.That(Id, Is.EqualTo(-1));
				Name = "Unknown";
				Assert.That(Name, Is.EqualTo("Unknown"));
				IPublic pub = this;
				pub.Id = -2;
				Assert.That(pub.Id, Is.EqualTo(-2));
				pub.Name = "Unknown2";
				Assert.That(pub.Name, Is.EqualTo("Unknown2"));
			}
		}

		[Serializable]
		public abstract class AbstractTestClass : IPublic
		{
			protected AbstractTestClass()
			{
				Id = -1;
				Assert.That(Id, Is.Zero);
				Name = "Unknown";
				Assert.That(Name, Is.Null);
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

		[Serializable]
		public abstract class ClassWithGenericNonVirtualMethod : IWithGenericMethod
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
			public T CopyTo<T>() where T : class, IWithGenericMethod
			{
				return null;
			}
		}

		public interface IWithGenericMethod
		{
			T CopyTo<T>() where T : class, IWithGenericMethod;
		}

		[Serializable]
		public class ClassWithInitProperties
		{
			public virtual int Id { get; init; }
			public virtual string Name { get; init; }
		}

		[Test]
		public void VerifyProxyForClassWithInternalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(InternalInterfaceTestClass).FullName, typeof(InternalInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);

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
				null, null, null, false);

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
		public void VerifyProxyForInterfaceWithEqualsAndGetHashCode()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(IWithEqualsAndGetHashCode).FullName,
				typeof(object),
				new HashSet<System.Type> {typeof(IWithEqualsAndGetHashCode), typeof(INHibernateProxy)},
				null, null, null, false);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IWithEqualsAndGetHashCode>());
					var proxyType = proxy.GetType();
					var proxyMap = proxyType.GetInterfaceMap(typeof(IWithEqualsAndGetHashCode));
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.Property("Name").EqualTo("Equals").And.Property("IsPublic").EqualTo(true),
						"Equals is not implicitly implemented");
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.Property("Name").EqualTo("GetHashCode").And.Property("IsPublic").EqualTo(true),
						"GetHashCode is not implicitly implemented");
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
				null, null, null, true);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<PublicInterfaceTestClass>());
					var proxyType = proxy.GetType();
					var proxyMap = proxyType.GetInterfaceMap(typeof(IPublic));
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.EqualTo(proxyType.GetProperty(nameof(PublicInterfaceTestClass.Name)).GetMethod),
						"Name getter does not implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.EqualTo(proxyType.GetProperty(nameof(PublicInterfaceTestClass.Name)).SetMethod),
						"Name setter does not implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.EqualTo(proxyType.GetProperty(nameof(PublicInterfaceTestClass.Id)).GetMethod),
						"Id setter does not implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.One.EqualTo(proxyType.GetProperty(nameof(PublicInterfaceTestClass.Id)).SetMethod),
						"Id setter does not implement IPublic");

					// Check interface and implicit implementations do both call the delegated state
					var state = new PublicInterfaceTestClass { Id = 5, Name = "State" };
					proxy.HibernateLazyInitializer.SetImplementation(state);
					var ent = (PublicInterfaceTestClass) proxy;
					IPublic pub = ent;
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
				null, null, null, true);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<PublicExplicitInterfaceTestClass>());
					var proxyType = proxy.GetType();
					Assert.That(proxyType.GetMethod($"get_{nameof(IPublic.Name)}"), Is.Null, "get Name is implicitly implemented");
					Assert.That(proxyType.GetMethod($"set_{nameof(IPublic.Name)}"), Is.Null, "set Name is implicitly implemented");
					Assert.That(proxyType.GetMethod($"get_{nameof(IPublic.Id)}"), Is.Null, "get Id is implicitly implemented");
					Assert.That(proxyType.GetMethod($"set_{nameof(IPublic.Id)}"), Is.Null, "set Id is implicitly implemented");

					// Check explicit implementation
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
		public void VerifyProxyForClassWithExplicitInterfaceWithSameMembers()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(PublicExplicitInterfaceWithSameMembersTestClass).FullName,
				typeof(PublicExplicitInterfaceWithSameMembersTestClass),
				new HashSet<System.Type> {typeof(INHibernateProxy)},
				null, null, null, true);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<IPublic>());
					Assert.That(proxy, Is.InstanceOf<PublicExplicitInterfaceWithSameMembersTestClass>());
					var proxyType = proxy.GetType();
					var proxyMap = proxyType.GetInterfaceMap(typeof(IPublic));
					Assert.That(
						proxyMap.TargetMethods,
						Has.None.EqualTo(proxyType.GetProperty(nameof(PublicExplicitInterfaceWithSameMembersTestClass.Name)).GetMethod),
						"class Name getter does implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.None.EqualTo(proxyType.GetProperty(nameof(PublicExplicitInterfaceWithSameMembersTestClass.Name)).SetMethod),
						"class Name setter does implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.None.EqualTo(proxyType.GetProperty(nameof(PublicExplicitInterfaceWithSameMembersTestClass.Id)).GetMethod),
						"class Id setter does implement IPublic");
					Assert.That(
						proxyMap.TargetMethods,
						Has.None.EqualTo(proxyType.GetProperty(nameof(PublicExplicitInterfaceWithSameMembersTestClass.Id)).SetMethod),
						"class Id setter does implement IPublic");

					// Check interface and implicit implementations do both call the delegated state
					var state = new PublicExplicitInterfaceWithSameMembersTestClass();
					IPublic pubState = state;
					state.Id = 5;
					state.Name = "State";
					pubState.Id = 10;
					pubState.Name = "State2";
					proxy.HibernateLazyInitializer.SetImplementation(state);
					var entity = (PublicExplicitInterfaceWithSameMembersTestClass) proxy;
					IPublic pubEntity = entity;
					Assert.That(entity.Id, Is.EqualTo(5), "Id member");
					Assert.That(entity.Name, Is.EqualTo("State"), "Name member");
					Assert.That(pubEntity.Id, Is.EqualTo(10), "Id from interface");
					Assert.That(pubEntity.Name, Is.EqualTo("State2"), "Name from interface");

					entity.Id = 15;
					entity.Name = "Test";
					pubEntity.Id = 20;
					pubEntity.Name = "Test2";
					Assert.That(entity.Id, Is.EqualTo(15), "entity.Id");
					Assert.That(state.Id, Is.EqualTo(15), "state.Id");
					Assert.That(entity.Name, Is.EqualTo("Test"), "entity.Name");
					Assert.That(state.Name, Is.EqualTo("Test"), "state.Name");
					Assert.That(pubEntity.Id, Is.EqualTo(20), "pubEntity.Id");
					Assert.That(pubState.Id, Is.EqualTo(20), "pubState.Id");
					Assert.That(pubEntity.Name, Is.EqualTo("Test2"), "pubEntity.Name");
					Assert.That(pubState.Name, Is.EqualTo("Test2"), "pubState.Name");
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
				null,
				true);

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
				null, null, null, true);

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
		public void VerifyProxyForInternalClass()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(InternalTestClass).FullName,
				typeof(InternalTestClass),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null, null, null, true);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<InternalTestClass>());

					Assert.That(factory.GetFieldInterceptionProxy(), Is.InstanceOf<InternalTestClass>());

#if NETFX
				});
#endif
		}

		[Test(Description = "GH2619")]
		public void VerifyProxyForClassWithGenericNonVirtualMethod()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(ClassWithGenericNonVirtualMethod).FullName,
				typeof(ClassWithGenericNonVirtualMethod),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null, null, null, true);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<ClassWithGenericNonVirtualMethod>());

					Assert.That(factory.GetFieldInterceptionProxy(), Is.InstanceOf<ClassWithGenericNonVirtualMethod>());

#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyProxyForClassWithInitProperties()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(ClassWithInitProperties).FullName,
				typeof(ClassWithInitProperties),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null, null, null, true);

#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var proxy = factory.GetProxy(1, null);
					Assert.That(proxy, Is.Not.Null);
					Assert.That(proxy, Is.InstanceOf<ClassWithInitProperties>());

					Assert.That(factory.GetFieldInterceptionProxy(), Is.InstanceOf<ClassWithInitProperties>());

#if NETFX
				});
#endif
		}

		[Test]
		public void InitializedProxyStaysInitializedAfterDeserialization()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
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
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
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
			factory.PostInstantiate(typeof(PublicInterfaceTestClass).FullName, typeof(PublicInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
			var proxy = (PublicInterfaceTestClass) factory.GetFieldInterceptionProxy();
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
			factory.PostInstantiate(typeof(CustomSerializationClass).FullName, typeof(CustomSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
			var proxy = (CustomSerializationClass) factory.GetFieldInterceptionProxy();
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
			factory.PostInstantiate(typeof(CustomExplicitSerializationClass).FullName, typeof(CustomExplicitSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
			var proxy = (CustomExplicitSerializationClass) factory.GetFieldInterceptionProxy();
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
			factory.PostInstantiate(typeof(InternalInterfaceTestClass).FullName, typeof(InternalInterfaceTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy();
					Assert.That(fieldProxy, Is.InstanceOf<InternalInterfaceTestClass>());
#if NETFX
				});
#endif
		}

		[Test]
		public void VerifyFieldInterceptorProxyWithISerializableEntity()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(CustomSerializationClass).FullName, typeof(CustomSerializationClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null, true);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy();
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
				null, null, null, false);
#if NETFX
			VerifyGeneratedAssembly(
				() =>
				{
#endif
					var fieldProxy = factory.GetFieldInterceptionProxy();
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

#if NETCOREAPP3_1_OR_GREATER
		public interface IWithStaticMethods
		{
			// C# 8
			static void StaticMethod()
			{
			}

#if NET7_0_OR_GREATER
			// C# 11
			static abstract void StaticAbstractMethod();

			// C# 11
			static virtual void StaticVirtualMethod()
			{
			}
#endif
		}

		public class ClassWithStaticInterfaceMethods : IWithStaticMethods
		{
			public static void StaticAbstractMethod()
			{
			}
		}
		
		[Test(Description = "GH3295")]
		public void VerifyProxyForClassWithStaticInterfaceMethod()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(
				typeof(ClassWithStaticInterfaceMethods).FullName,
				typeof(ClassWithStaticInterfaceMethods),
				new HashSet<System.Type> { typeof(INHibernateProxy) },
				null, null, null, true);

			var proxy = factory.GetProxy(1, null);
			Assert.That(proxy, Is.Not.Null);
			Assert.That(proxy, Is.InstanceOf<ClassWithStaticInterfaceMethods>());

			Assert.That(factory.GetFieldInterceptionProxy(), Is.InstanceOf<ClassWithStaticInterfaceMethods>());
		}
#endif
		
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
