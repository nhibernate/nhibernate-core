using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	[TestFixture]
	public class NativeSqlCollectionLoaderFixture : BugTestCase
	{
		protected virtual bool WithQueryCache => false;

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.UseQueryCache, WithQueryCache.ToString());
		}

		#region Tests - <return-join>

		[Test]
		public void LoadElementsWithWithSimpleHbmAliasInjection()
		{
			string[] routes = CreateRoutes();
			Country country = LoadCountryWithNativeSQL(CreateCountry(routes), "LoadCountryRoutesWithSimpleHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That(country.Routes, Is.EquivalentTo(routes));
		}

		[Test]
		public void LoadElementsWithExplicitColumnMappings()
		{
			string[] routes = CreateRoutes();
			Country country = LoadCountryWithNativeSQL(CreateCountry(routes), "LoadCountryRoutesWithCustomAliases");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Routes, Is.EquivalentTo(routes));
		}

		[Test]
		public void LoadCompositeElementsWithWithSimpleHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithSimpleHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That(country.Statistics.Keys, Is.EquivalentTo(stats.Keys), "Keys");
			Assert.That(country.Statistics.Values, Is.EquivalentTo(stats.Values), "Elements");
		}

		[Test]
		public void LoadCompositeElementsWithWithComplexHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithComplexHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That(country.Statistics.Keys, Is.EquivalentTo(stats.Keys), "Keys");
			Assert.That(country.Statistics.Values, Is.EquivalentTo(stats.Values), "Elements");
		}

		[Test]
		public void LoadCompositeElementsWithWithCustomAliases()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithCustomAliases");

			Assert.That(country, Is.Not.Null);
			Assert.That(country.Statistics.Keys, Is.EquivalentTo(stats.Keys), "Keys");
			Assert.That(country.Statistics.Values, Is.EquivalentTo(stats.Values), "Elements");
		}

		[Test]
		public void LoadEntitiesWithWithSimpleHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var query = session.GetNamedQuery("LoadCountryCitiesWithSimpleHbmAliasInjection")
								   .SetString("country_code", country.Code)
								   .SetCacheable(WithQueryCache);
				var c = query.UniqueResult<Country>();
				if (WithQueryCache)
					// Re-get it for obtaining it from cache.
					c = query.UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}
		}

		[Test]
		public void LoadEntitiesWithComplexHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var query = session.GetNamedQuery("LoadCountryCitiesWithComplexHbmAliasInjection")
								   .SetString("country_code", country.Code)
								   .SetCacheable(WithQueryCache);
				var c = query.UniqueResult<Country>();
				if (WithQueryCache)
					// Re-get it for obtaining it from cache.
					c = query.UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}
		}

		[Test]
		public void LoadEntitiesWithExplicitColumnMappings()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var query = session.GetNamedQuery("LoadCountryCitiesWithCustomAliases")
								   .SetString("country_code", country.Code)
								   .SetCacheable(WithQueryCache);
				var c = query.UniqueResult<Country>();
				if (WithQueryCache)
					// Re-get it for obtaining it from cache.
					c = query.UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}
		}

		[Test]
		public void NativeQueryWithUnresolvedHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Assert.That(
				() => LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithFaultyHbmAliasInjection"),
				Throws.InstanceOf<QueryException>());
		}

		private Country LoadCountryWithNativeSQL(Country country, string queryName)
		{
			// Ensure country is saved and session cache is empty to force from now on the reload of all 
			// persistence objects from the database.
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(country);
					tx.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				var query = session.GetNamedQuery(queryName).SetString("country_code", country.Code).SetCacheable(WithQueryCache);
				var result = query.UniqueResult<Country>();
				if (WithQueryCache)
					// Get it from cache by re-executing.
					result = query.UniqueResult<Country>();
				return result;
			}
		}

		#endregion

		#region Tests - <load-collection>

		[Test]
		public void LoadElementCollectionWithCustomLoader()
		{
			Assume.That(WithQueryCache, Is.False, "This test does not use a cacheable query.");
			string[] routes = CreateRoutes();
			Country country = CreateCountry(routes);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c = session.Get<Country>(country.Code);
				Assert.That(c, Is.Not.Null, "country");
				Assert.That(c.Routes, Is.EquivalentTo(routes), "country.Routes");
			}
		}

		[Test]
		public void LoadCompositeElementCollectionWithCustomLoader()
		{
			Assume.That(WithQueryCache, Is.False, "This test does not use a cacheable query.");
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = CreateCountry(stats);
			Save(country);
			using (ISession session = OpenSession())
			{
				var a = session.Get<Area>(country.Code);
				Assert.That(a, Is.Not.Null, "area");
				Assert.That(a.Statistics.Keys, Is.EquivalentTo(stats.Keys), "area.Keys");
				Assert.That(a.Statistics.Values, Is.EquivalentTo(stats.Values), "area.Elements");
			}
		}

		[Test]
		public void LoadEntityCollectionWithCustomLoader()
		{
			Assume.That(WithQueryCache, Is.False, "This test does not use a cacheable query.");
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c = session.Get<Country>(country.Code);

				Assert.That(c, Is.Not.Null, "country");
				Assert.That(c.Cities, Is.EquivalentTo(cities), "country.Cities");
			}
		}

		private void Save<TArea>(TArea area) where TArea : Area
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(area);
					tx.Commit();
				}
			}
		}

		#endregion

		#region Tests - corner cases to verify backwards compatibility of NH-1612 patch

		[Test]
		public void NativeUpdateQueryWithoutResults()
		{
			Assume.That(Dialect, Is.InstanceOf<MsSql2000Dialect>(), "This does not apply to {0}", Dialect);
			Assume.That(WithQueryCache, Is.False, "This test does not use a cacheable query.");
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.GetNamedQuery("UpdateQueryWithoutResults").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		[Test]
		public void NativeScalarQueryWithoutResults()
		{
			Assume.That(Dialect, Is.InstanceOf<MsSql2000Dialect>(), "This does not apply to {0}", Dialect);
			Assume.That(WithQueryCache, Is.False, "This test does not use a cacheable query.");
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					// Native SQL Query outcome is not validated against <return-*> 
					// resultset declarations.
					session.GetNamedQuery("ScalarQueryWithDefinedResultsetButNoResults").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		[Test]
		public void NativeScalarQueryWithUndefinedResultset()
		{
			if (!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore("This does not apply to {0}", Dialect);
			}
			using (ISession session = OpenSession())
			{
				using (session.BeginTransaction())
				{
					// Native SQL Query outcome is not validated against <return-*> 
					// resultset declarations.
					var query = session.GetNamedQuery("ScalarQueryWithUndefinedResultset")
									   .SetReadOnly(WithQueryCache);
					var result = query.UniqueResult<int>();
					if (WithQueryCache)
						result = query.UniqueResult<int>();
					Assert.That(result, Is.EqualTo(1));
				}
			}
		}

		[Test]
		public void NativeScalarQueryWithDefinedResultset()
		{
			if (!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore("This does not apply to {0}", Dialect);
			}
			using (ISession session = OpenSession())
			{
				using (session.BeginTransaction())
				{
					// Native SQL Query outcome is not validated against <return-*> 
					// resultset declarations.
					var query = session.GetNamedQuery("ScalarQueryWithDefinedResultset")
									   .SetReadOnly(WithQueryCache);
					var result = query.UniqueResult<int>();
					if (WithQueryCache)
						result = query.UniqueResult<int>();
					Assert.That(result, Is.EqualTo(2));
				}
			}
		}

		#endregion

		#region cleanup

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from City");
				session.Delete("from Country");
				tx.Commit();
			}
		}

		#endregion

		#region Factory methods

		private static Country CreateCountry()
		{
			const string COUNTRY_CODE = "WL";
			const string COUNTRY_NAME = "Wonderland";
			return new Country(COUNTRY_CODE, COUNTRY_NAME);
		}

		private static Country CreateCountry(params string[] routes)
		{
			Country country = CreateCountry();
			foreach (var route in routes)
			{
				country.Routes.Add(route);
			}
			return country;
		}

		private static Country CreateCountry(params City[] cities)
		{
			Country country = CreateCountry();
			foreach (var city in cities)
			{
				city.SetParent(country);
			}
			return country;
		}

		private static Country CreateCountry(IDictionary<int, AreaStatistics> statistics)
		{
			Country country = CreateCountry();
			foreach (var pair in statistics)
			{
				country.Statistics.Add(pair);
			}
			return country;
		}

		private static string[] CreateRoutes()
		{
			return new[] { "Yellow Road", "Muddy Path" };
		}

		private static City[] CreateCities()
		{
			return new[] { new City("EMR", "Emerald City"), new City("GLD", "Golden Town"), new City("NTH", "North End") };
		}

		private static IDictionary<int, AreaStatistics> CreateStatistics()
		{
			var archimedes = new Person("Archimedes");
			var archibald = new Person("Archibald");
			var amy = new Person("Amy");
			return new Dictionary<int, AreaStatistics>
					   {
						   {
							   1850,
							   new AreaStatistics {CitizenCount = 10000, GDP = new MonetaryValue("USD", 20000), Reporter = archimedes}
							   },
						   {
							   1900,
							   new AreaStatistics {CitizenCount = 20000, GDP = new MonetaryValue("USD", 50000), Reporter = archibald}
							   },
						   {1950, new AreaStatistics {CitizenCount = 40000, GDP = new MonetaryValue("USD", 125000)}},
						   {2000, new AreaStatistics {CitizenCount = 80000, GDP = new MonetaryValue("USD", 500000), Reporter = amy}},
					   };
		}

		#endregion
	}

	[TestFixture]
	public class CachedNativeSqlCollectionLoaderFixture : NativeSqlCollectionLoaderFixture
	{
		protected override bool WithQueryCache => true;
	}
}
