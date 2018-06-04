using System;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Proxy;
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
			var entry2 = new ProxyCacheEntry(typeof(Entity1), System.Type.EmptyTypes);
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
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(ISerializable) });
			// Interfaces order inverted on purpose: must be supported.
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(ISerializable), typeof(INHibernateProxy) });
			Assert.IsTrue(entry1.Equals(entry2));
			Assert.IsTrue(entry2.Equals(entry1));
		}

		// Non reg test case
		[Test]
		public void InterfaceEqualityWithLotOfUnordererdAndDupInterfaces()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(ISerializable), typeof(IType), typeof(IDisposable), typeof(IFilter) });
			// Interfaces order inverted and duplicated on purpose: must be supported.
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(IType), typeof(ISerializable), typeof(IFilter), typeof(IDisposable), typeof(IType), typeof(IFilter), typeof(INHibernateProxy) });
			Assert.IsTrue(entry1.Equals(entry2));
			Assert.IsTrue(entry2.Equals(entry1));
		}

		[Test]
		public void InterfaceInequality()
		{
			var entry1 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(INHibernateProxy), typeof(ISerializable) });
			var entry2 = new ProxyCacheEntry(typeof(Entity1), new[] { typeof(ISerializable) });
			TweakEntry(entry2, entry1.GetHashCode());
			Assert.IsFalse(entry1.Equals(entry2));
			Assert.IsFalse(entry2.Equals(entry1));
		}
	}
}
