using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	[TestFixture, Ignore("Not fixed yet.")]
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
			
			// cleanup
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete(country);
					tx.Commit();
				}
			}
		}

		[Test]
		public void LoadElementsWithExplicitColumnMappings()
		{
			string[] routes = CreateRoutes();
			Country country = LoadCountryWithNativeSQL(CreateCountry(routes), "LoadCountryRoutesWithCustomAliases");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Routes, Is.EquivalentTo(routes));
			// cleanup
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete(country);
					tx.Commit();
				}
			}
		}

		[Test]
		public void LoadCompositeElementsWithWithSimpleHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithSimpleHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");
			// cleanup
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete(country);
					tx.Commit();
				}
			}
		}

		[Test]
		public void LoadCompositeElementsWithWithComplexHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithComplexHbmAliasInjection");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");
		}

		[Test]
		public void LoadCompositeElementsWithWithCustomAliases()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithCustomAliases");

			Assert.That(country, Is.Not.Null);
			Assert.That((ICollection) country.Statistics.Keys, Is.EquivalentTo((ICollection) stats.Keys), "Keys");
			Assert.That((ICollection) country.Statistics.Values, Is.EquivalentTo((ICollection) stats.Values), "Elements");
		}

		[Test]
		public void LoadEntitiesWithWithSimpleHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = LoadCountryWithNativeSQL(CreateCountry(cities), "LoadCountryCitiesWithSimpleHbmAliasInjection");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Cities, Is.EquivalentTo(cities));
		}

		[Test]
		public void LoadEntitiesWithComplexHbmAliasInjection()
		{
			City[] cities = CreateCities();
			Country country = LoadCountryWithNativeSQL(CreateCountry(cities), "LoadCountryCitiesWithComplexHbmAliasInjection");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Cities, Is.EquivalentTo(cities));
		}

		[Test]
		public void LoadEntitiesWithExplicitColumnMappings()
		{
			City[] cities = CreateCities();
			Country country = LoadCountryWithNativeSQL(CreateCountry(cities), "LoadCountryCitiesWithCustomAliases");
			Assert.That(country, Is.Not.Null);
			Assert.That(country.Cities, Is.EquivalentTo(cities));
		}

		[Test, ExpectedException(typeof (QueryException))]
		public void NativeQueryWithUnresolvedHbmAliasInjection()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			LoadCountryWithNativeSQL(CreateCountry(stats), "LoadAreaStatisticsWithFaultyHbmAliasInjection");
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
			Country c = SaveAndReload(country);
			Assert.That(c, Is.Not.Null, "country");
			Assert.That(c.Routes, Is.EquivalentTo(routes), "country.Routes");

		}

		[Test]
		public void LoadCompositeElementCollectionWithCustomLoader()
		{
			IDictionary<int, AreaStatistics> stats = CreateStatistics();
			Country country = CreateCountry(stats);
			Area a = SaveAndReload(country);
			Assert.That(a, Is.Not.Null, "area");
			Assert.That((ICollection)a.Statistics.Keys, Is.EquivalentTo((ICollection)stats.Keys), "area.Keys");
			Assert.That((ICollection)a.Statistics.Values, Is.EquivalentTo((ICollection)stats.Values), "area.Elements");
		}

		[Test]
		public void LoadEntityCollectionWithCustomLoader()
		{
			City[] cities = CreateCities();
			Country country = CreateCountry(cities);
			Country c = SaveAndReload(country);
			Assert.That(c, Is.Not.Null, "country");
			Assert.That(c.Cities, Is.EquivalentTo(cities), "country.Cities");
		}

		private TArea SaveAndReload<TArea>(TArea area) where TArea : Area
		{
			//Ensure country is saved and session cache is empty to force from now on the reload of all 
			//persistence objects from the database.
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(area);
					tx.Commit();
				}

			}
			using (ISession session = OpenSession())
			{
				return session.Get<TArea>(area.Code);
			}
		}

		#endregion

		#region Tests - corner cases to verify backwards compatibility of NH-1612 patch

		[Test]
		public void NativeUpdateQueryWithoutResults()
		{
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