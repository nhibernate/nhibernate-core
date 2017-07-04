using System.Collections.Concurrent;
using System.Reflection;
using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3954
{
	[TestFixture, Explicit("Demonstrates bug impact on cache, but which tests will fail is a bit unpredictable")]
	public class ProxyCacheFixture
	{
		private ProxyCache _cache;
		private ConcurrentDictionary<ProxyCacheEntry, System.Type> _internalCache;
		private int _hashCode1;
		private int _hashCode2;

		private static readonly FieldInfo InternalCacheField =
			typeof(ProxyCache).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);

		[SetUp]
		public void SetUp()
		{
			_cache = new ProxyCache();

			_internalCache = (ConcurrentDictionary<ProxyCacheEntry, System.Type>)InternalCacheField.GetValue(null);

			_cache.StoreProxyType(typeof(Entity1FakeProxy).GetTypeInfo(), typeof(Entity1));
			_cache.StoreProxyType(typeof(Entity2FakeProxy).GetTypeInfo(), typeof(Entity2), typeof(INHibernateProxy));
			_cache.StoreProxyType(typeof(Entity3FakeProxy).GetTypeInfo(), typeof(Entity3));
			_cache.StoreProxyType(typeof(Entity4FakeProxy).GetTypeInfo(), typeof(Entity4), typeof(IProxy));
			_cache.StoreProxyType(typeof(Entity5FakeProxy).GetTypeInfo(), typeof(Entity5), typeof(INHibernateProxy), typeof(IProxy));

			// Artificially inject other entries with same hashcodes
			_hashCode1 = new ProxyCacheEntry(typeof(Entity1), null).GetHashCode();
			Inject(new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy) }), _hashCode1, typeof(Entity1FakeProxy2));
			Inject(new ProxyCacheEntry(typeof(Entity3), null), _hashCode1, typeof(Entity3FakeProxy2));

			_hashCode2 = new ProxyCacheEntry(typeof(Entity2), new[] { typeof(INHibernateProxy) }).GetHashCode();
			Inject(new ProxyCacheEntry(typeof(Entity2), null), _hashCode2, typeof(Entity2FakeProxy2));
			Inject(new ProxyCacheEntry(typeof(Entity4), new[] { typeof(IProxy) }), _hashCode2, typeof(Entity4FakeProxy2));
			Inject(new ProxyCacheEntry(typeof(Entity5), new[] { typeof(INHibernateProxy), typeof(IProxy) }), _hashCode2, typeof(Entity5FakeProxy2));
		}

		private void Inject(ProxyCacheEntry entryToTweak, int hashcode, System.Type result)
		{
			TweakEntry(entryToTweak, hashcode);
			_internalCache[entryToTweak] = result;
		}

		private static readonly FieldInfo HashCodeField =
			typeof(ProxyCacheEntry).GetField("_hashCode", BindingFlags.Instance | BindingFlags.NonPublic);

		/// <summary>
		/// Allows to simulate a hashcode collision. Issue would be unpractical to test otherwise.
		/// Hashcode collision must be supported for avoiding unexpected and hard to reproduce failures.
		/// </summary>
		private void TweakEntry(ProxyCacheEntry entryToTweak, int hashcode)
		{
			// Though hashCode is a readonly field, this works at the time of this writing. If it starts breaking and cannot be fixed,
			// ignore those tests or throw them away.
			HashCodeField.SetValue(entryToTweak, hashcode);
		}

		[Test]
		public void ProxyCacheEntity1FakeProxy()
		{
			var result = _cache.GetProxyType(typeof(Entity1));
			Assert.AreEqual(typeof(Entity1FakeProxy), result);
		}

		[Test]
		public void ProxyCacheEntity1FakeProxy2()
		{
			var entry = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy) });
			TweakEntry(entry, _hashCode1);
			var result = _internalCache[entry];
			Assert.AreEqual(typeof(Entity1FakeProxy2), result);
		}

		[Test]
		public void ProxyCacheEntity2FakeProxy()
		{
			var result = _cache.GetProxyType(typeof(Entity2), typeof(INHibernateProxy));
			Assert.AreEqual(typeof(Entity2FakeProxy), result);
		}

		[Test]
		public void ProxyCacheEntity2FakeProxy2()
		{
			var entry = new ProxyCacheEntry(typeof(Entity2), null);
			TweakEntry(entry, _hashCode2);
			var result = _internalCache[entry];
			Assert.AreEqual(typeof(Entity2FakeProxy2), result);
		}

		[Test]
		public void ProxyCacheEntity3FakeProxy()
		{
			var result = _cache.GetProxyType(typeof(Entity3));
			Assert.AreEqual(typeof(Entity3FakeProxy), result);
		}

		[Test]
		public void ProxyCacheEntity3FakeProxy2()
		{
			var entry = new ProxyCacheEntry(typeof(Entity3), null);
			TweakEntry(entry, _hashCode1);
			var result = _internalCache[entry];
			Assert.AreEqual(typeof(Entity3FakeProxy2), result);
		}

		[Test]
		public void ProxyCacheEntity4FakeProxy()
		{
			var result = _cache.GetProxyType(typeof(Entity4), typeof(IProxy));
			Assert.AreEqual(typeof(Entity4FakeProxy), result);
		}

		[Test]
		public void ProxyCacheEntity4FakeProxy2()
		{
			var entry = new ProxyCacheEntry(typeof(Entity4), new[] { typeof(IProxy) });
			TweakEntry(entry, _hashCode2);
			var result = _internalCache[entry];
			Assert.AreEqual(typeof(Entity4FakeProxy2), result);
		}

		[Test]
		public void ProxyCacheEntity5FakeProxy()
		{
			var result = _cache.GetProxyType(typeof(Entity5), typeof(IProxy), typeof(INHibernateProxy));
			Assert.AreEqual(typeof(Entity5FakeProxy), result);
		}

		[Test]
		public void ProxyCacheEntity5FakeProxy2()
		{
			// Interfaces order inverted on purpose: must be supported.
			var entry = new ProxyCacheEntry(typeof(Entity5), new[] { typeof(IProxy), typeof(INHibernateProxy) });
			TweakEntry(entry, _hashCode2);
			var result = _internalCache[entry];
			Assert.AreEqual(typeof(Entity5FakeProxy2), result);
		}

		[Test]
		public void ProxyCacheNone()
		{
			// Beware not testing the lookup failure of any combination, even tweaked, actually added in cache.
			// (Otherwise the test may starts failing unexpectedly sometimes, as the original bug ...)
			// This one was not added in anyway.
			TypeInfo result;
			Assert.IsFalse(_cache.TryGetProxyType(typeof(Entity2), new[] { typeof(IProxy) }, out result));
		}
	}
}