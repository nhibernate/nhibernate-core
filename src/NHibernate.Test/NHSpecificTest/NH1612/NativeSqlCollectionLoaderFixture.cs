using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	[TestFixture]
	public class NativeSqlCollectionLoaderFixture : BugTestCase
	{
		#region Tests - <return-join>

		[Test]
		public void LoadElementsWithWithSimpleHbmAliasInjection()
		{
			string[] routes = CreateRoutes();
			Country country = LoadCountryWithNativeSQL(CreateCountry(routes), "LoadCountryRoutesWithSimpleHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That(country.Routes, Is.EquivalentTo(routes));

			Cleanup();
		}

		[Test]
		public void LoadElementsWithExplicitColumnMappings()
		{
			string[] routes = CreateRoutes();
			Country country = LoadCountryWithNativeSQL(CreateCountry(routes), "LoadCountryRoutesWithCustomAliases");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Routes, Is.EquivalentTo(routes));
			Cleanup();
		}

		[Test]
		public void LoadCompositeElementsWithWithSimpleHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithSimpleHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");
			CleanupWithPersons();
		}

		[Test]
		public void LoadCompositeElementsWithWithComplexHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithComplexHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");
			CleanupWithPersons();
		}

		[Test]
		public void LoadCompositeElementsWithWithCustomAliases()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithCustomAliases");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");

			CleanupWithPersons();
		}

		[Test]
		public void LoadEntitiesWithWithSimpleHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c =
					session.GetNamedQuery("LoadCountryCitiesWithSimpleHbmAliasInjection").SetString("country_code", country.Code).
						UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}
			CleanupWithCities();
		}

		[Test]
		public void LoadEntitiesWithComplexHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c =
					session.GetNamedQuery("LoadCountryCitiesWithComplexHbmAliasInjection").SetString("country_code", country.Code).
						UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}
			CleanupWithCities();
		}

		[Test]
		public void LoadEntitiesWithExplicitColumnMappings()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c =
					session.GetNamedQuery("LoadCountryCitiesWithCustomAliases").SetString("country_code", country.Code).
						UniqueResult<Country>();
				Assert.That(c, Is.Not.Null);
				Assert.That(c.Cities, Is.EquivalentTo(cities));
			}

			// cleanup
			CleanupWithCities();
		}

		[Test]
		public void NativeQueryWithUnresolvedHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			try
			{
				LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithFaultyHbmAliasInjection");
				Assert.Fail("Expected exception");
			}
			catch(QueryException)
			{
				// ok
			}
			finally
			{
				// cleanup
				CleanupWithPersons();
			}
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
				return session.GetNamedQuery(queryName).SetString("country_code", country.Code).UniqueResult<Country>();
			}
		}

		#endregion

		#region Tests - <load-collection>

		[Test]
		public void LoadElementCollectionWithCustomLoader()
		{
			string[] routes = CreateRoutes();
			Country country = CreateCountry(routes);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c = session.Get<Country>(country.Code);
				Assert.That(c, Is.Not.Null, "country");
				Assert.That(c.Routes, Is.EquivalentTo(routes), "country.Routes");
			}
			Cleanup();
		}

		[Test]
		public void LoadCompositeElementCollectionWithCustomLoader()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = CreateCountry(stats);
			Save(country);
			using (ISession session = OpenSession())
			{
				var a = session.Get<Area>(country.Code);
				Assert.That(a, Is.Not.Null, "area");
				Assert.That((ICollection) a.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "area.Keys");
				Assert.That((ICollection) a.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "area.Elements");
			}
			CleanupWithPersons();
		}

		[Test]
		public void LoadEntityCollectionWithCustomLoader()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Save(country);
			using (ISession session = OpenSession())
			{
				var c = session.Get<Country>(country.Code);

				Assert.That(c, Is.Not.Null, "country");
				Assert.That(c.Cities, Is.EquivalentTo(cities), "country.Cities");
			}
			CleanupWithCities();
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
			if(!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore("This does not apply to {0}", Dialect);
			}
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
			if (!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore("This does not apply to {0}", Dialect);
			}
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
					var result = session.GetNamedQuery("ScalarQueryWithUndefinedResultset").UniqueResult<int>();
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
					var result = session.GetNamedQuery("ScalarQueryWithDefinedResultset").UniqueResult<int>();
					Assert.That(result, Is.EqualTo(2));
				}
			}
		}

		#endregion

		#region cleanup

		private void Cleanup()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Country");
					tx.Commit();
				}
			}
		}

		private void CleanupWithPersons()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					session.Delete("from Country");
					tx.Commit();
				}
			}
		}

		private void CleanupWithCities()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from City");
					session.Delete("from Country");
					tx.Commit();
				}
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
			return new[] {"Yellow Road", "Muddy Path"};
		}

		private static City[] CreateCities()
		{
			return new[] {new City("EMR", "Emerald City"), new City("GLD", "Golden Town"), new City("NTH", "North End")};
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
}