using System;
using System.Reflection;
using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3954
{
	[TestFixture]
	public class EqualsFixture
	{
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

		// Non reg test case
		[Test]
		public void TypeEquality()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), null);
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new System.Type[0]);
			Assert.IsTrue(entry1.Equals(entry2));
			Assert.IsTrue(entry2.Equals(entry1));
		}

		[Test]
		public void TypeInequality()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), null);
			var entry2 = new ProxyCacheEntry(typeof(Entity2), null);
			TweakEntry(entry2, entry1.GetHashCode());
			Assert.IsFalse(entry1.Equals(entry2));
			Assert.IsFalse(entry2.Equals(entry1));
		}

		// Non reg test case
		[Test]
		public void InterfaceEquality()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(IProxy) });
			// Interfaces order inverted on purpose: must be supported.
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(IProxy), typeof(INHibernateProxy) });
			Assert.IsTrue(entry1.Equals(entry2));
			Assert.IsTrue(entry2.Equals(entry1));
		}

		// Non reg test case
		[Test]
		public void InterfaceEqualityWithLotOfUnordererdAndDupInterfaces()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(IProxy), typeof(IType), typeof(IDisposable), typeof(IFilter) });
			// Interfaces order inverted and duplicated on purpose: must be supported.
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(IType), typeof(IProxy), typeof(IFilter), typeof(IDisposable), typeof(IType), typeof(IFilter), typeof(INHibernateProxy) });
			Assert.IsTrue(entry1.Equals(entry2));
			Assert.IsTrue(entry2.Equals(entry1));
		}

		[Test]
		public void InterfaceInequality()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(IProxy) });
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(IProxy) });
			TweakEntry(entry2, entry1.GetHashCode());
			Assert.IsFalse(entry1.Equals(entry2));
			Assert.IsFalse(entry2.Equals(entry1));
		}
	}
}