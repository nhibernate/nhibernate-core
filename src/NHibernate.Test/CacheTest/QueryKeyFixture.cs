using System.Collections;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class QueryKeyFixture : TestCase
	{
		private readonly SqlString SqlAll =
			new SqlString("select entitywith0_.id as id0_, entitywith0_.Description as Descript2_0_, entitywith0_.Value as Value0_ from EntityWithFilters entitywith0_");

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] { "CacheTest.EntityWithFilters.hbm.xml" };

		[Test]
		public void EqualityWithFilters()
		{
			QueryKeyFilterDescLikeToCompare(out var qk, out var qk1, true);
			Assert.That(qk, Is.EqualTo(qk1), "Like");

			QueryKeyFilterDescValueToCompare(out qk, out qk1, true);
			Assert.That(qk, Is.EqualTo(qk1), "Value");
		}

		private void QueryKeyFilterDescLikeToCompare(out QueryKey qk, out QueryKey qk1, bool sameValue)
		{
			const string filterName = "DescriptionLike";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk =  new FilterKey(f);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);

			var f1 = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f1.SetParameter("pLike", sameValue ? "so%" : "%ing");
			var fk1 = new FilterKey(f1);
			fks = new HashSet<FilterKey> { fk1 };
			qk1 = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);
		}

		private void QueryKeyFilterDescValueToCompare(out QueryKey qk, out QueryKey qk1, bool sameValue)
		{
			const string filterName = "DescriptionEqualAndValueGT";

			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			var fk = new FilterKey(f);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);

			var f1 = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f1.SetParameter("pDesc", "something").SetParameter("pValue", sameValue ? 10 : 11);
			var fk1 = new FilterKey(f1);
			fks = new HashSet<FilterKey> { fk1 };
			qk1 = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);
		}

		[Test]
		public void NotEqualityWithFilters()
		{
			QueryKeyFilterDescLikeToCompare(out var qk, out var qk1, false);
			Assert.That(qk, Is.Not.EqualTo(qk1), "qk & qk1");

			QueryKeyFilterDescValueToCompare(out var qvk, out var qvk1, false);
			Assert.That(qvk, Is.Not.EqualTo(qvk1), "qvk & qvk1");

			Assert.That(qk, Is.Not.EqualTo(qvk), "qk & qvk");
			Assert.That(qk1, Is.Not.EqualTo(qvk1), "qk1 & qvk1");
		}

		[Test]
		public void HashCodeWithFilters()
		{
			QueryKeyFilterDescLikeToCompare(out var qk, out var qk1, true);
			Assert.That(qk.GetHashCode(), Is.EqualTo(qk1.GetHashCode()), "Like");

			QueryKeyFilterDescValueToCompare(out qk, out qk1, true);
			Assert.That(qk.GetHashCode(), Is.EqualTo(qk1.GetHashCode()), "Value");
		}

		[Test]
		public void NotEqualHashCodeWithFilters()
		{
			// GetHashCode semantic does not guarantee no collision may ever occur, but the algorithm should
			// generates different hashcodes for similar but inequal cases. These tests check that cache keys
			// for a query generated for different parameters values are no more equal.
			QueryKeyFilterDescLikeToCompare(out var qk, out var qk1, false);
			Assert.That(qk.GetHashCode(), Is.Not.EqualTo(qk1.GetHashCode()), "qk & qk1");

			QueryKeyFilterDescValueToCompare(out var qvk, out var qvk1, false);
			Assert.That(qvk.GetHashCode(), Is.Not.EqualTo(qvk1.GetHashCode()), "qvk & qvk1");

			Assert.That(qk.GetHashCode(), Is.Not.EqualTo(qvk.GetHashCode()), "qk & qvk");
			Assert.That(qk1.GetHashCode(), Is.Not.EqualTo(qvk1.GetHashCode()), "qk1 & qvk1");
		}

		[Test]
		public void ToStringWithFilters()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(f);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			var qk = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Does.Contain($"filters: ['{fk}']"), "Like");

			filterName = "DescriptionEqualAndValueGT";
			f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(f);
			fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Does.Contain($"filters: ['{fk}']"), "Value");
		}

		[Test]
		public void ToStringWithMoreFilters()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(f);

			filterName = "DescriptionEqualAndValueGT";
			var fv = new FilterImpl(Sfi.GetFilterDefinition(filterName));
			fv.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			var fvk = new FilterKey(fv);

			ISet<FilterKey> fks = new HashSet<FilterKey> { fk, fvk };
			var qk = new QueryKey(Sfi, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Does.Contain($"filters: ['{fk}', '{fvk}']"));
		}
	}
}
