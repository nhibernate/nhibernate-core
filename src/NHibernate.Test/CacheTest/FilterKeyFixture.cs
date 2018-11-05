using System.Collections;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class FilterKeyFixture: TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "CacheTest.EntityWithFilters.hbm.xml" };

		[Test]
		public void ToStringIncludeAll()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(f);
			Assert.That(fk.ToString(), Is.EqualTo("FilterKey[DescriptionLike{'pLike'='so%'}]"), "Like");

			filterName = "DescriptionEqualAndValueGT";
			f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(f);
			Assert.That(fk.ToString(), Is.EqualTo("FilterKey[DescriptionEqualAndValueGT{'pDesc'='something', 'pValue'='10'}]"), "Value");
		}

		[Test]
		public void Equality()
		{
			FilterDescLikeToCompare(out var fk, out var fk1, true);
			Assert.That(fk, Is.EqualTo(fk1), "Like");

			FilterDescValueToCompare(out fk, out fk1, true);
			Assert.That(fk, Is.EqualTo(fk1), "Value");

			FilterValueInToCompare(out fk, out fk1, true);
			Assert.That(fk, Is.EqualTo(fk1), "In");
		}

		private void FilterDescLikeToCompare(out FilterKey fk, out FilterKey fk1, bool sameValue)
		{
			const string filterName = "DescriptionLike";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			fk = new FilterKey(f);

			var f1 = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f1.SetParameter("pLike", sameValue ? "so%" : "%ing");
			fk1 = new FilterKey(f1);
		}

		private void FilterDescValueToCompare(out FilterKey fk, out FilterKey fk1, bool sameValue)
		{
			const string filterName = "DescriptionEqualAndValueGT";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(f);

			var f1 = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f1.SetParameter("pDesc", "something").SetParameter("pValue", sameValue ? 10 : 11);
			fk1 = new FilterKey(f1);
		}

		private void FilterValueInToCompare(out FilterKey fk, out FilterKey fk1, bool sameValue)
		{
			const string filterName = "ValueIn";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameterList("pIn", new HashSet<int> { 10, 11 });
			fk = new FilterKey(f);

			var f1 = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f1.SetParameterList("pIn", sameValue ? (ICollection<int>)new [] { 10, 11 } : new HashSet<int> { 10, 12 });
			fk1 = new FilterKey(f1);
		}

		[Test]
		public void NotEquality()
		{
			FilterDescLikeToCompare(out var fk, out var fk1, false);
			Assert.That(fk, Is.Not.EqualTo(fk1), "fk & fk1");

			FilterDescValueToCompare(out var fvk, out var fvk1, false);
			Assert.That(fvk, Is.Not.EqualTo(fvk1), "fvk & fvk1");

			FilterValueInToCompare(out var fik, out var fik1, false);
			Assert.That(fik, Is.Not.EqualTo(fik1), "fik & fik1");

			Assert.That(fk, Is.Not.EqualTo(fvk), "fk & fvk");
			Assert.That(fk1, Is.Not.EqualTo(fvk1), "fk1 & fvk1");
			Assert.That(fvk, Is.Not.EqualTo(fik), "fvk & fik");
			Assert.That(fvk1, Is.Not.EqualTo(fik1), "fvk1 & fik1");
		}

		[Test]
		public void HashCode()
		{
			FilterDescLikeToCompare(out var fk, out var fk1, true);
			Assert.That(fk.GetHashCode(), Is.EqualTo(fk1.GetHashCode()), "Like");

			FilterDescValueToCompare(out fk, out fk1, true);
			Assert.That(fk.GetHashCode(), Is.EqualTo(fk1.GetHashCode()), "Value");

			FilterValueInToCompare(out fk, out fk1, true);
			Assert.That(fk.GetHashCode(), Is.EqualTo(fk1.GetHashCode()), "In");
		}

		[Test]
		public void NotEqualHashCode()
		{
			// GetHashCode semantic does not guarantee no collision may ever occur, but the algorithm should
			// generates different hashcodes for similar but inequal cases. These tests check that cache keys
			// for a query generated for different parameters values are no more equal.
			FilterDescLikeToCompare(out var fk, out var fk1, false);
			Assert.That(fk.GetHashCode(), Is.Not.EqualTo(fk1.GetHashCode()), "fk & fk1");

			FilterDescValueToCompare(out var fvk, out var fvk1, false);
			Assert.That(fvk.GetHashCode(), Is.Not.EqualTo(fvk1.GetHashCode()), "fvk & fvk1");

			FilterValueInToCompare(out var fik, out var fik1, false);
			Assert.That(fik.GetHashCode(), Is.Not.EqualTo(fik1.GetHashCode()), "fik & fik1");

			Assert.That(fk.GetHashCode(), Is.Not.EqualTo(fvk.GetHashCode()), "fk & fvk");
			Assert.That(fk1.GetHashCode(), Is.Not.EqualTo(fvk1.GetHashCode()), "fk1 & fvk1");
			Assert.That(fvk.GetHashCode(), Is.Not.EqualTo(fik.GetHashCode()), "fvk & fik");
			Assert.That(fvk1.GetHashCode(), Is.Not.EqualTo(fik1.GetHashCode()), "fvk1 & fik1");
		}
	}
}
