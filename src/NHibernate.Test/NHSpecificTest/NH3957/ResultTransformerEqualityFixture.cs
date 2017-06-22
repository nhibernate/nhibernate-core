using System.Collections.Generic;
using System.Reflection;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3957
{
	[TestFixture]
	public class ResultTransformerEqualityFixture
	{
		/// <summary>
		/// Allows to simulate a hashcode collision. Issue would be unpractical to test otherwise.
		/// Hashcode collision must be supported for avoiding unexpected and hard to reproduce failures.
		/// </summary>
		private void TweakHashcode(System.Type transformerToTweak, object hasher)
		{
			var hasherTargetField = transformerToTweak.GetField("Hasher", BindingFlags.Static | BindingFlags.NonPublic);
			if (!_hasherBackup.ContainsKey(transformerToTweak))
				_hasherBackup.Add(transformerToTweak, hasherTargetField.GetValue(null));

			// Though hasher is a readonly field, this works at the time of this writing. If it starts breaking and cannot be fixed,
			// ignore those tests or throw them away.
			hasherTargetField.SetValue(null, hasher);
		}

		private Dictionary<System.Type, object> _hasherBackup = new Dictionary<System.Type, object>();

		[SetUp]
		public void Setup()
		{
			var hasherForAll = typeof(AliasToEntityMapResultTransformer)
				.GetField("Hasher", BindingFlags.Static | BindingFlags.NonPublic)
				.GetValue(null);
			TweakHashcode(typeof(DistinctRootEntityResultTransformer), hasherForAll);
			TweakHashcode(typeof(PassThroughResultTransformer), hasherForAll);
			TweakHashcode(typeof(RootEntityResultTransformer), hasherForAll);
			TweakHashcode(typeof(ToListResultTransformer), hasherForAll);
		}

		[TearDown]
		public void TearDown()
		{
			// Restore those types hashcode. (Avoid impacting perf of other tests, avoid second level query cache
			// issues if it was holding cached entries (but would mean some tests have not cleaned up properly).)
			foreach(var backup in _hasherBackup)
			{
				TweakHashcode(backup.Key, backup.Value);
			}
		}

		// Non reg test case
		[Test]
		public void AliasToEntityMapEquality()
		{
			var transf1 = new AliasToEntityMapResultTransformer();
			var transf2 = new AliasToEntityMapResultTransformer();

			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void AliasToEntityMapAndDistinctRootEntityInequality()
		{
			var transf1 = new AliasToEntityMapResultTransformer();
			var transf2 = new DistinctRootEntityResultTransformer();

			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void DistinctRootEntityEquality()
		{
			var transf1 = new DistinctRootEntityResultTransformer();
			var transf2 = new DistinctRootEntityResultTransformer();

			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void PassThroughEquality()
		{
			var transf1 = new PassThroughResultTransformer();
			var transf2 = new PassThroughResultTransformer();

			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void PassThroughAndRootEntityInequality()
		{
			var transf1 = new PassThroughResultTransformer();
			var transf2 = new RootEntityResultTransformer();
			
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void RootEntityEquality()
		{
			var transf1 = new RootEntityResultTransformer();
			var transf2 = new RootEntityResultTransformer();

			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void RootEntityAndToListInequality()
		{
			var transf1 = new RootEntityResultTransformer();
			var transf2 = new ToListResultTransformer();
			
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void ToListEquality()
		{
			var transf1 = new ToListResultTransformer();
			var transf2 = new ToListResultTransformer();

			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}
	}
}