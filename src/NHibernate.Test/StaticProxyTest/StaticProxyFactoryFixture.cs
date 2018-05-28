using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.StaticProxyTest
{
	public class StaticProxyFactoryFixture
	{
		internal interface ISomething
		{
			int Id { get; }
		}

		public class TestClass : ISomething
		{
			public virtual int Id { get; set; }
		}

		[Serializable]
		public class SimpleTestClass
		{
			public virtual int Id { get; set; }
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
		public void InitializedProxyStaysInitializedAfterDeserialization()
		{
			TestsContext.AssumeSystemTypeIsSerializable();

			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(2, null);
			Assert.That(proxy, Is.Not.Null, "proxy");
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.False, "proxy already initialized after creation");
			Assert.That(proxy.HibernateLazyInitializer, Is.Not.Null, "HibernateLazyInitializer");

			var impl = new SimpleTestClass { Id = 2 };
			proxy.HibernateLazyInitializer.SetImplementation(impl);
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.True, "proxy not initialized after setting implementation");

			var serializer = new BinaryFormatter();
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
			TestsContext.AssumeSystemTypeIsSerializable();

			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(SimpleTestClass).FullName, typeof(SimpleTestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(2, null);
			Assert.That(proxy, Is.Not.Null, "proxy");
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.False, "proxy already initialized after creation");

			var serializer = new BinaryFormatter();
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
	}
}
