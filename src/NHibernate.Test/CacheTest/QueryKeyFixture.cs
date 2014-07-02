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

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "CacheTest.EntityWithFilters.xml" }; }
		}

		[Test]
		public void EqualityWithFilters()
		{
			QueryKey qk, qk1;
			QueryKeyFilterDescLikeToCompare(out qk, out qk1);
			Assert.That(qk, Is.EqualTo(qk1));

			QueryKeyFilterDescValueToCompare(out qk, out qk1);
			Assert.That(qk, Is.EqualTo(qk1));
		}

		private void QueryKeyFilterDescLikeToCompare(out QueryKey qk, out QueryKey qk1)
		{
			const string filterName = "DescriptionLike";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk =  new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);

			var f1 = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f1.SetParameter("pLike", "%ing");
			var fk1 = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			fks = new HashSet<FilterKey> { fk1 };
			qk1 = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);
		}

		private void QueryKeyFilterDescValueToCompare(out QueryKey qk, out QueryKey qk1)
		{
			const string filterName = "DescriptionEqualAndValueGT";

			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			var fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);

			var f1 = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f1.SetParameter("pDesc", "something").SetParameter("pValue", 11);
			var fk1 = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			fks = new HashSet<FilterKey> { fk1 };
			qk1 = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);
		}

		[Test]
		public void NotEqualityWithFilters()
		{
			QueryKey qk, qk1;
			QueryKeyFilterDescLikeToCompare(out qk, out qk1);

			QueryKey qvk, qvk1;
			QueryKeyFilterDescValueToCompare(out qvk, out qvk1);

			Assert.That(qk, Is.Not.EqualTo(qvk));
			Assert.That(qk1, Is.Not.EqualTo(qvk1));
		}

		[Test]
		public void HashCodeWithFilters()
		{
			QueryKey qk, qk1;
			QueryKeyFilterDescLikeToCompare(out qk, out qk1);
			Assert.That(qk.GetHashCode(), Is.EqualTo(qk1.GetHashCode()));

			QueryKeyFilterDescValueToCompare(out qk, out qk1);
			Assert.That(qk.GetHashCode(), Is.EqualTo(qk1.GetHashCode()));
		}

		[Test]
		public void NotEqualHashCodeWithFilters()
		{
			QueryKey qk, qk1;
			QueryKeyFilterDescLikeToCompare(out qk, out qk1);

			QueryKey qvk, qvk1;
			QueryKeyFilterDescValueToCompare(out qvk, out qvk1);

			Assert.That(qk.GetHashCode(), Is.Not.EqualTo(qvk.GetHashCode()));
			Assert.That(qk1.GetHashCode(), Is.Not.EqualTo(qvk1.GetHashCode()));
		}

		[Test]
		public void ToStringWithFilters()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			ISet<FilterKey> fks = new HashSet<FilterKey> { fk };
			var qk = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Is.StringContaining(string.Format("filters: ['{0}']",fk)));

			filterName = "DescriptionEqualAndValueGT";
			f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			fks = new HashSet<FilterKey> { fk };
			qk = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Is.StringContaining(string.Format("filters: ['{0}']", fk)));
		}

		[Test]
		public void ToStringWithMoreFilters()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);

			filterName = "DescriptionEqualAndValueGT";
			var fv = new FilterImpl(sessions.GetFilterDefinition(filterName));
			fv.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			var fvk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);

			ISet<FilterKey> fks = new HashSet<FilterKey> { fk, fvk };
			var qk = new QueryKey(sessions, SqlAll, new QueryParameters(), fks, null);
			Assert.That(qk.ToString(), Is.StringContaining(string.Format("filters: ['{0}', '{1}']", fk, fvk)));
		}
	}
}