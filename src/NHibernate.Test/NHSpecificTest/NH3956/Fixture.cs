using System;
using System.Reflection;
using NHibernate.Engine.Query.Sql;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3956
{
	[TestFixture]
	public class Fixture
	{
		private static readonly FieldInfo HashCodeField =
			typeof(NativeSQLQuerySpecification).GetField("hashCode", BindingFlags.Instance | BindingFlags.NonPublic);

		/// <summary>
		/// Allows to simulate a hashcode collision. Issue would be unpractical to test otherwise.
		/// Hashcode collision must be supported for avoiding unexpected and hard to reproduce failures.
		/// </summary>
		private void TweakHashcode(NativeSQLQuerySpecification specToTweak, int hashCode)
		{
			// Though hashCode is a readonly field, this works at the time of this writing. If it starts breaking and cannot be fixed,
			// ignore those tests or throw them away.
			HashCodeField.SetValue(specToTweak, hashCode);
		}

		[Test]
		public void NativeSQLQuerySpecificationEqualityOnQuery()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null, null);
			// Empty spaces array should be equivalent to null. Maybe results too but current implementation does not handle this.
			var spec2 = new NativeSQLQuerySpecification("select blah", null, Array.Empty<string>());

			Assert.IsTrue(spec1.Equals(spec2));
			Assert.IsTrue(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnQuery()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null, null);
			var spec2 = new NativeSQLQuerySpecification("select blargh", null, null);

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationEqualityOnReturns()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal)
				},
				null);
			var spec2 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					// Aliases ordering matters, do not get them mixed. (But type does not matter, I guess this means
					// a case with a same query having sames aliases but different types is never supposed to happen
					// Note that Hibernate does test other properties as of this writing:
					// https://github.com/hibernate/hibernate-orm/blob/master/hibernate-core/src/main/java/org/hibernate/engine/query/spi/sql/NativeSQLQueryScalarReturn.java
					// And same on other INativeSQLQueryReturn implementations.
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal)
				},
				// Empty spaces array should be equivalent to null.
				Array.Empty<string>());

			Assert.IsTrue(spec1.Equals(spec2));
			Assert.IsTrue(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnNullReturn()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null, null);
			var spec2 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character)
				},
				null);

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnReturnContents()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal)
				},
				null);
			var spec2 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias3", NHibernateUtil.Decimal)
				},
				null);

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnReturnLengthes()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
				},
				null);
			var spec2 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal)
				},
				null);

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnReturnOrderings()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character),
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal)
				},
				null);
			var spec2 = new NativeSQLQuerySpecification("select blah",
				new[]
				{
					new NativeSQLQueryScalarReturn("alias2", NHibernateUtil.Decimal),
					new NativeSQLQueryScalarReturn("alias1", NHibernateUtil.Character)
				},
				null);

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationEqualityOnSpaces()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });
			var spec2 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });

			Assert.IsTrue(spec1.Equals(spec2));
			Assert.IsTrue(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnNullSpace()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null, null);
			var spec2 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnSpaceContents()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });
			var spec2 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space3" });

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnSpaceLengthes()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });
			var spec2 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2", "space3" });

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}

		[Test]
		public void NativeSQLQuerySpecificationInequalityOnSpaceOrderings()
		{
			var spec1 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space1", "space2" });
			var spec2 = new NativeSQLQuerySpecification("select blah", null,
				new[] { "space2", "space1" });

			TweakHashcode(spec2, spec1.GetHashCode());
			Assert.IsFalse(spec1.Equals(spec2));
			Assert.IsFalse(spec2.Equals(spec1));
		}
	}
}
