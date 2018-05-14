﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class CollectionHelperFixture
	{
		#region Bag

		[Test]
		public void BagComparedToSelfShouldBeEqual()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.BagEquals(c1, c1), Is.True);
		}

		[Test]
		public void BagComparedToNullShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.BagEquals(c1, null), Is.False);
			Assert.That(CollectionHelper.BagEquals(null, c1), Is.False);
		}

		[Test]
		public void BagNullComparedToNullShouldBeEqual()
		{
			Assert.That(CollectionHelper.BagEquals<string>(null, null), Is.True);
		}

		[Test]
		public void BagsWithSameContentShouldBeEqual()
		{
			var c1 = new List<string> { "1", "2", "3", "4", "2" };
			var c2 = new List<string> { "1", "2", "3", "4", "2" };
			Assert.That(CollectionHelper.BagEquals(c1, c2), Is.True);
		}

		[Test]
		public void BagsWithSameCountButDistinctValuesShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			var c2 = new List<string> { "1", "2", "3", "3" };
			Assert.That(CollectionHelper.BagEquals(c1, c2), Is.False);
		}

		[Test]
		public void BagsWithSameElementsButDistinctOrderShouldBeEqual()
		{
			var c1 = new List<string> { "1", "2", "3", "4", "2" };
			var c2 = new List<string> { "2", "1", "2", "4", "3" };
			Assert.That(CollectionHelper.BagEquals(c1, c2), Is.True);
		}

		[Test]
		public void BagsWithoutSameCountShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "2", "1" };
			var c2 = new List<string> { "1", "2" };
			Assert.That(CollectionHelper.BagEquals(c1, c2), Is.False);
			Assert.That(CollectionHelper.BagEquals(c2, c1), Is.False);
		}

		#endregion

		#region Dictionary/Map

		[Test]
		public void MapComparedToSelfShouldBeEqual()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, d1), Is.True);
		}

		[Test]
		public void MapComparedToNullShouldBeInequal()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, null), Is.False);
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(null, d1), Is.False);
		}

		[Test]
		public void MapNullComparedToNullShouldBeEqual()
		{
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(null, null), Is.True);
		}

		[Test]
		public void MapsWithSameContentShouldBeEqual()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, d2), Is.True);
		}

		// Fails till NH-3981 is fixed.
		[Test]
		public void MapsWithSameCountButDistinctKeysShouldBeInequal()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "4", "3" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, d2), Is.False);
		}

		[Test]
		public void MapsWithSameCountButDistinctValuesShouldBeInequal()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "3", "3" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, d2), Is.False);
		}

		[Test]
		public void MapsWithoutSameCountShouldBeInequal()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" } };
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d1, d2), Is.False);
			Assert.That(CollectionHelper.DictionaryEquals<string, string>(d2, d1), Is.False);
		}

		private static readonly MethodInfo KvpGetHashCode =
			typeof(CollectionHelper)
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
				.Where(m => m.Name == "GetHashCode")
				.Select(m => new { m, p = m.GetParameters() })
				.Where(mp => mp.p.Length == 1)
				.Select(mp => new { mp.m, pt = mp.p[0].ParameterType })
				.Single(
					mpt => mpt.pt.IsGenericType &&
						!mpt.pt.IsGenericTypeDefinition &&
						mpt.pt.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
						mpt.pt.GenericTypeArguments.Length == 1 &&
						mpt.pt.GenericTypeArguments[0].IsGenericType &&
						!mpt.pt.GenericTypeArguments[0].IsGenericTypeDefinition &&
						mpt.pt.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
				.m;

		[Test]
		public void MapsWithSameContentShouldHaveSameHashCode()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var kvpGetHashCode = KvpGetHashCode.MakeGenericMethod(typeof(string), typeof(string));
			Assert.That(
				kvpGetHashCode.Invoke(null, new object[] { d1 }),
				Is.EqualTo(kvpGetHashCode.Invoke(null, new object[] { d2 })));
		}

		// Failure of following tests is not an error from GetHashCode semantic viewpoint, but it causes it
		// to be potentially inefficients for usages in dictionnaries or hashset.

		[Test]
		public void MapsWithSameCountButDistinctKeysShouldNotHaveSameHashCode()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "4", "3" } };
			var kvpGetHashCode = KvpGetHashCode.MakeGenericMethod(typeof(string), typeof(string));
			Assert.That(
				kvpGetHashCode.Invoke(null, new object[] { d1 }),
				Is.Not.EqualTo(kvpGetHashCode.Invoke(null, new object[] { d2 })));
		}

		[Test]
		public void MapsWithSameCountButDistinctValuesShouldNotHaveSameHashCode()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" }, { "3", "3" } };
			var kvpGetHashCode = KvpGetHashCode.MakeGenericMethod(typeof(string), typeof(string));
			Assert.That(
				kvpGetHashCode.Invoke(null, new object[] { d1 }),
				Is.Not.EqualTo(kvpGetHashCode.Invoke(null, new object[] { d2 })));
		}

		[Test]
		public void MapsWithoutSameCountShouldNotHaveSameHashCode()
		{
			var d1 = new Dictionary<string, string> { { "1", "2" }, { "3", "4" } };
			var d2 = new Dictionary<string, string> { { "1", "2" } };
			var kvpGetHashCode = KvpGetHashCode.MakeGenericMethod(typeof(string), typeof(string));
			Assert.That(
				kvpGetHashCode.Invoke(null, new object[] { d1 }),
				Is.Not.EqualTo(kvpGetHashCode.Invoke(null, new object[] { d2 })));
		}

		#endregion

		#region Set

		[Test]
		public void SetComparedToSelfShouldBeEqual()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SetEquals(c1, c1), Is.True);
		}

		[Test]
		public void SetComparedToNullShouldBeInequal()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SetEquals(c1, null), Is.False);
			Assert.That(CollectionHelper.SetEquals(null, c1), Is.False);
		}

		[Test]
		public void SetNullComparedToNullShouldBeEqual()
		{
			Assert.That(CollectionHelper.SetEquals<string>(null, null), Is.True);
		}

		[Test]
		public void SetsWithSameContentShouldBeEqual()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			var c2 = new HashSet<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SetEquals(c1, c2), Is.True);
		}

		[Test]
		public void SetsWithSameCountButDistinctValuesShouldBeInequal()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			var c2 = new HashSet<string> { "1", "2", "3", "5" };
			Assert.That(CollectionHelper.SetEquals(c1, c2), Is.False);
		}

		[Test]
		public void SetsWithSameElementsButDistinctOrderShouldBeEqual()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			var c2 = new HashSet<string> { "1", "2", "4", "3" };
			Assert.That(CollectionHelper.SetEquals(c1, c2), Is.True);
		}

		[Test]
		public void SetsWithoutSameCountShouldBeInequal()
		{
			var c1 = new HashSet<string> { "1", "2", "3", "4" };
			var c2 = new HashSet<string> { "1", "2" };
			Assert.That(CollectionHelper.SetEquals(c1, c2), Is.False);
			Assert.That(CollectionHelper.SetEquals(c2, c1), Is.False);
		}

		#endregion

		#region List/Sequence

		[Test]
		public void SequenceComparedToSelfShouldBeEqual()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SequenceEquals(c1, c1), Is.True);
		}

		[Test]
		public void SequenceComparedToNullShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SequenceEquals(c1, null), Is.False);
			Assert.That(CollectionHelper.SequenceEquals(null, c1), Is.False);
		}

		[Test]
		public void SequenceNullComparedToNullShouldBeEqual()
		{
			Assert.That(CollectionHelper.SequenceEquals<string>(null, null), Is.True);
		}

		[Test]
		public void SequencesWithSameContentShouldBeEqual()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			var c2 = new List<string> { "1", "2", "3", "4" };
			Assert.That(CollectionHelper.SequenceEquals(c1, c2), Is.True);
		}

		[Test]
		public void SequencesWithSameCountButDistinctValuesShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			var c2 = new List<string> { "1", "2", "3", "3" };
			Assert.That(CollectionHelper.SequenceEquals(c1, c2), Is.False);
		}

		[Test]
		public void SequencesWithSameElementsButDistinctOrderShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			var c2 = new List<string> { "1", "2", "4", "3" };
			Assert.That(CollectionHelper.SequenceEquals(c1, c2), Is.False);
		}

		[Test]
		public void SequencesWithoutSameCountShouldBeInequal()
		{
			var c1 = new List<string> { "1", "2", "3", "4" };
			var c2 = new List<string> { "1", "2" };
			Assert.That(CollectionHelper.SequenceEquals(c1, c2), Is.False);
			Assert.That(CollectionHelper.SequenceEquals(c2, c1), Is.False);
		}

		#endregion
	}
}
