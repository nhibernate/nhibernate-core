using System.Collections;
using NHibernate.Cache;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class FilterKeyFixture: TestCase
	{
		protected override string MappingsAssembly
		{
			get{return "NHibernate.Test";}
		}

		protected override IList Mappings
		{
			get { return new[] { "CacheTest.EntityWithFilters.xml" }; }
		}

		[Test]
		public void ToStringIncludeAll()
		{
			string filterName = "DescriptionLike";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			var fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			Assert.That(fk.ToString(), Is.EqualTo("FilterKey[DescriptionLike{'pLike'='so%'}]"));

			filterName = "DescriptionEqualAndValueGT";
			f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
			Assert.That(fk.ToString(), Is.EqualTo("FilterKey[DescriptionEqualAndValueGT{'pDesc'='something', 'pValue'='10'}]"));
		}

		[Test]
		public void Equality()
		{
			// Equality is aware only by parameters names not values
			FilterKey fk, fk1;
			FilterDescLikeToCompare(out fk, out fk1);
			Assert.That(fk, Is.EqualTo(fk1));

			FilterDescValueToCompare(out fk, out fk1);
			Assert.That(fk, Is.EqualTo(fk1));
		}

		private void FilterDescLikeToCompare(out FilterKey fk, out FilterKey fk1)
		{
			const string filterName = "DescriptionLike";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pLike", "so%");
			fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);

			var f1 = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f1.SetParameter("pLike", "%ing");
			fk1 = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
		}

		private void FilterDescValueToCompare(out FilterKey fk, out FilterKey fk1)
		{
			const string filterName = "DescriptionEqualAndValueGT";
			var f = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f.SetParameter("pDesc", "something").SetParameter("pValue", 10);
			fk = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);

			var f1 = new FilterImpl(sessions.GetFilterDefinition(filterName));
			f1.SetParameter("pDesc", "something").SetParameter("pValue", 11);
			fk1 = new FilterKey(filterName, f.Parameters, f.FilterDefinition.ParameterTypes, EntityMode.Poco);
		}

		[Test]
		public void NotEquality()
		{
			FilterKey fk, fk1;
			FilterDescLikeToCompare(out fk, out fk1);

			FilterKey fvk, fvk1;
			FilterDescValueToCompare(out fvk, out fvk1);

			Assert.That(fk, Is.Not.EqualTo(fvk));
			Assert.That(fk1, Is.Not.EqualTo(fvk1));
		}

		[Test]
		public void HashCode()
		{
			// HashCode is aware only by parameters names not values (should work as Equal)
			FilterKey fk, fk1;
			FilterDescLikeToCompare(out fk, out fk1);
			Assert.That(fk.GetHashCode(), Is.EqualTo(fk1.GetHashCode()));

			FilterDescValueToCompare(out fk, out fk1);
			Assert.That(fk.GetHashCode(), Is.EqualTo(fk1.GetHashCode()));

		}

		[Test]
		public void NotEqualHashCode()
		{
			FilterKey fk, fk1;
			FilterDescLikeToCompare(out fk, out fk1);

			FilterKey fvk, fvk1;
			FilterDescValueToCompare(out fvk, out fvk1);

			Assert.That(fk.GetHashCode(), Is.Not.EqualTo(fvk.GetHashCode()));
			Assert.That(fk1.GetHashCode(), Is.Not.EqualTo(fvk1.GetHashCode()));
		}
	}
}