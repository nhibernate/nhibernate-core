using System.Collections.Generic;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3957
{
	[TestFixture]
	public class ResultTransformerEqualityFixture
	{
		public class CustomAliasToEntityMapResultTransformer : AliasToEntityMapResultTransformer { }
		public class CustomDistinctRootEntityResultTransformer : DistinctRootEntityResultTransformer { }
		public class CustomPassThroughResultTransformer : PassThroughResultTransformer { }
		public class CustomRootEntityResultTransformer : RootEntityResultTransformer { }
		
		// Non reg test case
		[Test]
		public void AliasToEntityMapEquality()
		{
			var transf1 = new AliasToEntityMapResultTransformer();
			var transf2 = new AliasToEntityMapResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(set.Count, Is.EqualTo(1));
			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void AliasToEntityMapAndDistinctRootEntityInequality()
		{
			var transf1 = new AliasToEntityMapResultTransformer();
			var transf2 = new CustomAliasToEntityMapResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(transf1.GetHashCode(), Is.EqualTo(transf2.GetHashCode()), "prerequisite");
			Assert.That(set.Count, Is.EqualTo(2));
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void DistinctRootEntityEquality()
		{
			var transf1 = new DistinctRootEntityResultTransformer();
			var transf2 = new DistinctRootEntityResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(set.Count, Is.EqualTo(1));
			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void DistinctRootEntityEqualityInequality()
		{
			var transf1 = new DistinctRootEntityResultTransformer();
			var transf2 = new CustomDistinctRootEntityResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(transf1.GetHashCode(), Is.EqualTo(transf2.GetHashCode()), "prerequisite");
			Assert.That(set.Count, Is.EqualTo(2));
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void PassThroughEquality()
		{
			var transf1 = new PassThroughResultTransformer();
			var transf2 = new PassThroughResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(set.Count, Is.EqualTo(1));
			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void PassThroughAndRootEntityInequality()
		{
			var transf1 = new PassThroughResultTransformer();
			var transf2 = new CustomPassThroughResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(transf1.GetHashCode(), Is.EqualTo(transf2.GetHashCode()), "prerequisite");
			Assert.That(set.Count, Is.EqualTo(2));
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void RootEntityEquality()
		{
			var transf1 = new RootEntityResultTransformer();
			var transf2 = new RootEntityResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(set.Count, Is.EqualTo(1));
			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}

		[Test]
		public void RootEntityAndToListInequality()
		{
			var transf1 = new RootEntityResultTransformer();
			var transf2 = new CustomRootEntityResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2, };

			Assert.That(transf1.GetHashCode(), Is.EqualTo(transf2.GetHashCode()), "prerequisite");
			Assert.That(set.Count, Is.EqualTo(2));
			Assert.IsFalse(transf1.Equals(transf2));
			Assert.IsFalse(transf2.Equals(transf1));
		}

		// Non reg test case
		[Test]
		public void ToListEquality()
		{
			var transf1 = new ToListResultTransformer();
			var transf2 = new ToListResultTransformer();
			HashSet<IResultTransformer> set = new HashSet<IResultTransformer>() { transf1, transf2 };

			Assert.That(set.Count, Is.EqualTo(1));
			Assert.IsTrue(transf1.Equals(transf2));
			Assert.IsTrue(transf2.Equals(transf1));
		}
	}
}
