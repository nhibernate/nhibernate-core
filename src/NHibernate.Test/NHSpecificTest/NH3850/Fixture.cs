using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Linq;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3850
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const string _searchName1 = "name";
		private const string _searchName2 = "name2";
		private const int _totalEntityCount = 10;
		private readonly DateTime _testDate = DateTime.Now;
		private readonly DateTimeOffset _testDateWithOffset = DateTimeOffset.Now;

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsSqlType(new SqlType(DbType.DateTimeOffset));
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			// Cannot handle DbType.DateTimeOffset via ODBC.
			return !(factory.ConnectionProvider.Driver.IsOdbcDriver());
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				var dateTime1 = _testDate.AddDays(-1);
				var dateTime2 = _testDate.AddDays(1);
				var dateTimeOffset1 = _testDateWithOffset.AddDays(-1);
				var dateTimeOffset2 = _testDateWithOffset.AddDays(1);
				Action<DomainClassBase> init1 = dc =>
				{
					dc.Id = 1;
					dc.Name = _searchName1;
					dc.Integer = 1;
					dc.Long = 1;
					dc.Decimal = 1;
					dc.Double = 1;
					dc.DateTime = dateTime1;
					dc.DateTimeOffset = dateTimeOffset1;
					dc.NonNullableDecimal = 1;
				};
				Action<DomainClassBase> init2 = dc =>
				{
					dc.Id = 2;
					dc.Name = _searchName2;
					dc.Integer = 2;
					dc.Long = 2;
					dc.Decimal = 2;
					dc.Double = 2;
					dc.DateTime = dateTime2;
					dc.DateTimeOffset = dateTimeOffset2;
					dc.NonNullableDecimal = 2;
				};

				DomainClassBase entity = new DomainClassBExtendedByA();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassBExtendedByA();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassCExtendedByD();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassCExtendedByD();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassE();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassE();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassGExtendedByH();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassGExtendedByH();
				init2(entity);
				session.Save(entity);
				entity = new DomainClassHExtendingG
				{
					Id = 3,
					Name = _searchName1,
					Integer = 3,
					Long = 3,
					Decimal = 3,
					Double = 3,
					DateTime = dateTime1,
					DateTimeOffset = dateTimeOffset1,
					NonNullableDecimal = 3
				};
				session.Save(entity);
				entity = new DomainClassHExtendingG
				{
					Id = 4,
					Name = _searchName2,
					Integer = 4,
					Long = 4,
					Decimal = 4,
					Double = 4,
					DateTime = dateTime2,
					DateTimeOffset = dateTimeOffset2,
					NonNullableDecimal = 4
				};
				session.Save(entity);

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				var hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AggregateGBase()
		{
			using (var session = OpenSession())
			{
				// This case should work because the aggregate is insensitive to ordering.
				var query = session.Query<DomainClassGExtendedByH>()
				                   .OrderBy(dc => dc.Id)
				                   .Select(dc => dc.Id);
				var result = query.Aggregate((p, n) => p + n);
				Assert.That(result, Is.EqualTo(10));
				var futureQuery = query.ToFutureValue(qdc => qdc.Aggregate((p, n) => p + n));
				Assert.That(futureQuery.Value, Is.EqualTo(10), "Future");
			}
		}

		// Failing case due to lack of polymorphic results ordering.
		[Test, Ignore("Polymorphic results sets are unioned without reordering, whatever the API")]
		public void AggregateGBaseOrderingMismatch()
		{
			using (var session = OpenSession())
			{
				// This case cannot work because the aggregate is sensitive to ordering, and NHibernate currently always order polymorphic queries by class names,
				// then only honors query ordering as secondary order criteria.
				var query = session.Query<DomainClassGExtendedByH>()
				                   .OrderByDescending(dc => dc.Id)
				                   .Select(dc => dc.Id.ToString());
				var result = query.Aggregate((p, n) => p + "," + n);
				// Currently yields "2,1,4,3" instead.
				Assert.That(result, Is.EqualTo("4,3,2,1"));
				var futureQuery = query.ToFutureValue(qdc => qdc.Aggregate((p, n) => p + "," + n));
				Assert.That(futureQuery.Value, Is.EqualTo("4,3,2,1"), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AggregateMutableSeedGBase()
		{
			using (var session = OpenSession())
			{
				// This case works because the ordering accidentally matches with classes ordering.
				// (And moreover, with current dataset, selected values are same whatever the classes.)
				var query = session.Query<DomainClassGExtendedByH>()
				                    .OrderBy(dc => dc.Id);
				var seed = new StringBuilder();
				var result = query.Aggregate(seed, (s, dc) => s.Append(dc.Name).Append(","));
				var expectedResult = _searchName1 + "," + _searchName2 + "," + _searchName1 + "," + _searchName2 + ",";
				Assert.That(result.ToString(), Is.EqualTo(expectedResult));
				// We are dodging another bug here: the seed is cached in query plan... So giving another seed to Future
				// keeps re-using the seed used for non future above.
				seed.Clear();
				var futureQuery = query.ToFutureValue(qdc => qdc.Aggregate(seed, (s, dc) => s.Append(dc.Name).Append(",")));
				Assert.That(futureQuery.Value.ToString(), Is.EqualTo(expectedResult), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AggregateSeedGBase()
		{
			using (var session = OpenSession())
			{
				// This case should work because the aggregate is insensitive to ordering.
				var query = session.Query<DomainClassGExtendedByH>()
				                   .OrderBy(dc => dc.Id);
				var result = query.Aggregate(5, (s, dc) => s + dc.Id);
				Assert.That(result, Is.EqualTo(15));
				var futureQuery = query.ToFutureValue(qdc => qdc.Aggregate(5, (s, dc) => s + dc.Id));
				Assert.That(futureQuery.Value, Is.EqualTo(15), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AllBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.False);
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AllCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.False);
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AllEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.False);
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AllFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AllGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.False);
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AllGBaseWithNameFilteredByName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>()
				                   .Where(dc => dc.Name == _searchName1);
				var result = query.All(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				var futureQuery = query.ToFutureValue(qdc => qdc.All(dc => dc.Name == _searchName1));
				Assert.That(futureQuery.Value, Is.True, "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Any();
				Assert.That(result, Is.True);
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Any(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.Any(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Any();
				Assert.That(result, Is.True);
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Any(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.Any(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Any();
				Assert.That(result, Is.True);
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Any(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.Any(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Any();
				Assert.That(result, Is.False);
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Any(dc => dc.Name == _searchName1);
				Assert.That(result, Is.False);
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.Any(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.False, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Any();
				Assert.That(result, Is.True);
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void AnyGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Any(dc => dc.Name == _searchName1);
				Assert.That(result, Is.True);
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.Any(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Any();
				Assert.That(result, Is.True);
				result = session.Query<object>().ToFutureValue(qdc => qdc.Any()).Value;
				Assert.That(result, Is.True, "Future");
			}
		}

		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageBBase()
		{
			Average<DomainClassBExtendedByA>(1.5m);
		}

		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageCBase()
		{
			Average<DomainClassCExtendedByD>(1.5m);
		}

		// Non-reg case
		[Test]
		public void AverageE()
		{
			Average<DomainClassE>(1.5m);
		}

		// Non-reg case
		[Test]
		public void AverageF()
		{
			Average<DomainClassF>(null);
		}

		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageGBase()
		{
			Average<DomainClassGExtendedByH>(2.5m);
		}

		private void Average<DC>(decimal? expectedResult) where DC : DomainClassBase
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var integ = dcQuery.Average(dc => dc.Integer);
				Assert.AreEqual(expectedResult, integ, "Integer average has failed");
				var futureInteg = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.Integer));
				Assert.That(futureInteg.Value, Is.EqualTo(expectedResult), "Future integer average has failed");

				var longInt = dcQuery.Average(dc => dc.Long);
				Assert.AreEqual(expectedResult, longInt, "Long integer average has failed");
				var futureLongInt = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.Long));
				Assert.That(futureLongInt.Value, Is.EqualTo(expectedResult), "Future long integer average has failed");

				var dec = dcQuery.Average(dc => dc.Decimal);
				Assert.AreEqual(expectedResult, dec, "Decimal average has failed");
				var futureDec = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.Decimal));
				Assert.That(futureDec.Value, Is.EqualTo(expectedResult), "Future decimal average has failed");

				var dbl = dcQuery.Average(dc => dc.Double);
				Assert.That(dbl.HasValue, Is.EqualTo(expectedResult.HasValue),"Double average has failed");
				if (expectedResult.HasValue)
					Assert.That(dbl.Value, Is.EqualTo(expectedResult).Within(0.001d), "Double average has failed");
				var futureDbl = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.Double));
				Assert.That(futureDbl.Value.HasValue, Is.EqualTo(expectedResult.HasValue),"Future double average has failed");
				if (expectedResult.HasValue)
					Assert.That(futureDbl.Value.Value, Is.EqualTo(expectedResult).Within(0.001d), "Future double average has failed");

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.That(() => nonNullableDecimal = dcQuery.Average(dc => dc.NonNullableDecimal), Throws.Nothing, "Non nullable decimal average has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Non nullable decimal average has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.NonNullableDecimal));
					Assert.That(() => nonNullableDecimal = futureNonNullableDec.Value, Throws.Nothing, "Future non nullable decimal average has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Future non nullable decimal average has failed");
				}
				else
				{
					Assert.That(() => dcQuery.Average(dc => dc.NonNullableDecimal),
					            // After fix
					            Throws.InstanceOf<InvalidOperationException>()
					                  // Before fix
					                  .Or.InnerException.InstanceOf<ArgumentNullException>(),
					            "Non nullable decimal average has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Average(dc => dc.NonNullableDecimal));
					Assert.That(() => futureNonNullableDec.Value,
					            Throws.InstanceOf<ArgumentNullException>(),
					            "Future non nullable decimal average has failed");
				}
			}
		}

		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Average(o => (int?)2);
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<object>().ToFutureValue(qdc => qdc.Average(o => (int?)2)).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Count();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Count(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.Count(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Count();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Count(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.Count(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Count();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Count(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.Count(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Count();
				Assert.That(result, Is.EqualTo(0));
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(0), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void CountFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Count(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(0));
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.Count(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(0), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Count();
				Assert.That(result, Is.EqualTo(4));
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(4), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Count(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.Count(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Count();
				Assert.That(result, Is.EqualTo(_totalEntityCount));
				result = session.Query<object>().ToFutureValue(qdc => qdc.Count()).Value;
				Assert.That(result, Is.EqualTo(_totalEntityCount), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultBBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassBExtendedByA>();
				DomainClassBExtendedByA result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassBExtendedByA>();
				DomainClassBExtendedByA result = null;
				Assert.That(() => result = query.FirstOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultCBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassCExtendedByD>();
				DomainClassCExtendedByD result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassCExtendedByD>();
				DomainClassCExtendedByD result = null;
				Assert.That(() => result = query.FirstOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultE()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassE>();
				DomainClassE result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultEWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassE>();
				DomainClassE result = null;
				Assert.That(() => result = query.FirstOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultF()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassF>();
				DomainClassF result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Null);
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Null, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultFWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassF>();
				DomainClassF result = null;
				Assert.That(() => result = query.FirstOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Null);
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Null, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultGBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				DomainClassGExtendedByH result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassGExtendedByH>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassGExtendedByH>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				DomainClassGExtendedByH result = null;
				Assert.That(() => result = query.FirstOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassGExtendedByH>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassGExtendedByH>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void FirstOrDefaultObject()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<object>();
				object result = null;
				Assert.That(() => result = query.FirstOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>());
				var futureQuery = query.ToFutureValue(qdc => qdc.FirstOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>(), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().LongCount();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().LongCount(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassBExtendedByA>().ToFutureValue(qdc => qdc.LongCount(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().LongCount();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().LongCount(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassCExtendedByD>().ToFutureValue(qdc => qdc.LongCount(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().LongCount();
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().LongCount(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(1));
				result = session.Query<DomainClassE>().ToFutureValue(qdc => qdc.LongCount(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().LongCount();
				Assert.That(result, Is.EqualTo(0));
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(0), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void LongCountFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().LongCount(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(0));
				result = session.Query<DomainClassF>().ToFutureValue(qdc => qdc.LongCount(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(0), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().LongCount();
				Assert.That(result, Is.EqualTo(4));
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(4), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().LongCount(dc => dc.Name == _searchName1);
				Assert.That(result, Is.EqualTo(2));
				result = session.Query<DomainClassGExtendedByH>().ToFutureValue(qdc => qdc.LongCount(dc => dc.Name == _searchName1)).Value;
				Assert.That(result, Is.EqualTo(2), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().LongCount();
				Assert.That(result, Is.EqualTo(_totalEntityCount));
				result = session.Query<object>().ToFutureValue(qdc => qdc.LongCount()).Value;
				Assert.That(result, Is.EqualTo(_totalEntityCount), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxBBase()
		{
			Max<DomainClassBExtendedByA>(2);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxCBase()
		{
			Max<DomainClassCExtendedByD>(2);
		}

		// Non-reg case
		[Test]
		public void MaxE()
		{
			Max<DomainClassE>(2);
		}

		// Non-reg case
		[Test]
		public void MaxF()
		{
			Max<DomainClassF>(null);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxGBase()
		{
			Max<DomainClassGExtendedByH>(4);
		}

		private void Max<DC>(int? expectedResult) where DC : DomainClassBase
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var name = dcQuery.Max(dc => dc.Name);
				Assert.That(name, Is.EqualTo(expectedResult.HasValue ? _searchName2 : null), "String max has failed");
				var futureName = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.Name));
				Assert.That(futureName.Value, Is.EqualTo(expectedResult.HasValue ? _searchName2 : null), "Future string max has failed");

				var integ = dcQuery.Max(dc => dc.Integer);
				Assert.That(integ, Is.EqualTo(expectedResult), "Integer max has failed");
				var futureInteg = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.Integer));
				Assert.That(futureInteg.Value, Is.EqualTo(expectedResult), "Future integer max has failed");

				var longInt = dcQuery.Max(dc => dc.Long);
				Assert.That(longInt, Is.EqualTo(expectedResult), "Long integer max has failed");
				var futureLongInt = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.Long));
				Assert.That(futureLongInt.Value, Is.EqualTo(expectedResult), "Future long integer max has failed");

				var dec = dcQuery.Max(dc => dc.Decimal);
				Assert.That(dec, Is.EqualTo(expectedResult), "Decimal max has failed");
				var futureDec = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.Decimal));
				Assert.That(futureDec.Value, Is.EqualTo(expectedResult), "Future decimal max has failed");

				var dbl = dcQuery.Max(dc => dc.Double);
				Assert.That(dbl.HasValue, Is.EqualTo(expectedResult.HasValue),"Double max has failed");
				if (expectedResult.HasValue)
					Assert.That(dbl.Value, Is.EqualTo(expectedResult).Within(0.001d), "Double max has failed");
				var futureDbl = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.Double));
				Assert.That(futureDbl.Value.HasValue, Is.EqualTo(expectedResult.HasValue),"Future double max has failed");
				if (expectedResult.HasValue)
					Assert.That(futureDbl.Value.Value, Is.EqualTo(expectedResult).Within(0.001d), "Future double max has failed");

				var date = dcQuery.Max(dc => dc.DateTime);
				var dateWithOffset = dcQuery.Max(dc => dc.DateTimeOffset);
				var futureDate = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.DateTime));
				var futureDateWithOffset = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.DateTimeOffset));
				if (expectedResult.HasValue)
				{
					Assert.That(date, Is.GreaterThan(_testDate), "DateTime max has failed");
					Assert.That(dateWithOffset, Is.GreaterThan(_testDateWithOffset), "DateTimeOffset max has failed");
					Assert.That(futureDate.Value, Is.GreaterThan(_testDate), "Future DateTime max has failed");
					Assert.That(futureDateWithOffset.Value, Is.GreaterThan(_testDateWithOffset), "Future DateTimeOffset max has failed");
				}
				else
				{
					Assert.That(date, Is.Null, "DateTime max has failed");
					Assert.That(dateWithOffset, Is.Null, "DateTimeOffset max has failed");
					Assert.That(futureDate.Value, Is.Null, "Future DateTime max has failed");
					Assert.That(futureDateWithOffset.Value, Is.Null, "Future DateTimeOffset max has failed");
				}

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.That(() => nonNullableDecimal = dcQuery.Max(dc => dc.NonNullableDecimal), Throws.Nothing, "Non nullable decimal max has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Non nullable decimal max has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.NonNullableDecimal));
					Assert.That(() => nonNullableDecimal = futureNonNullableDec.Value, Throws.Nothing, "Future non nullable decimal max has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Future non nullable decimal max has failed");
				}
				else
				{
					Assert.That(() => dcQuery.Max(dc => dc.NonNullableDecimal),
					            // After fix
					            Throws.InstanceOf<InvalidOperationException>()
					                  // Before fix
					                  .Or.InnerException.InstanceOf<ArgumentNullException>(),
					            "Non nullable decimal max has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.NonNullableDecimal));
					Assert.That(() => futureNonNullableDec.Value,
					            Throws.TargetInvocationException.And.InnerException.InstanceOf<InvalidOperationException>(),
					            "Future non nullable decimal max has failed");
				}
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MinBBase()
		{
			Min<DomainClassBExtendedByA>(1);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MinCBase()
		{
			Min<DomainClassCExtendedByD>(1);
		}

		// Non-reg case
		[Test]
		public void MinE()
		{
			Min<DomainClassE>(1);
		}

		// Non-reg case
		[Test]
		public void MinF()
		{
			Min<DomainClassF>(null);
		}

		// Non-reg case
		[Test]
		public void MinGBase()
		{
			Min<DomainClassGExtendedByH>(1);
		}

		private void Min<DC>(int? expectedResult) where DC : DomainClassBase
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var name = dcQuery.Min(dc => dc.Name);
				Assert.That(name, Is.EqualTo(expectedResult.HasValue ? _searchName1 : null), "String min has failed");
				var futureName = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.Name));
				Assert.That(futureName.Value, Is.EqualTo(expectedResult.HasValue ? _searchName1 : null), "Future string min has failed");

				var integ = dcQuery.Min(dc => dc.Integer);
				Assert.That(integ, Is.EqualTo(expectedResult), "Integer min has failed");
				var futureInteg = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.Integer));
				Assert.That(futureInteg.Value, Is.EqualTo(expectedResult), "Future integer min has failed");

				var longInt = dcQuery.Min(dc => dc.Long);
				Assert.That(longInt, Is.EqualTo(expectedResult), "Long integer min has failed");
				var futureLongInt = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.Long));
				Assert.That(futureLongInt.Value, Is.EqualTo(expectedResult), "Future long integer min has failed");

				var dec = dcQuery.Min(dc => dc.Decimal);
				Assert.That(dec, Is.EqualTo(expectedResult), "Decimal min has failed");
				var futureDec = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.Decimal));
				Assert.That(futureDec.Value, Is.EqualTo(expectedResult), "Future decimal min has failed");

				var dbl = dcQuery.Min(dc => dc.Double);
				Assert.That(dbl.HasValue, Is.EqualTo(expectedResult.HasValue),"Double min has failed");
				if (expectedResult.HasValue)
					Assert.That(dbl.Value, Is.EqualTo(expectedResult).Within(0.001d), "Double min has failed");
				var futureDbl = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.Double));
				Assert.That(futureDbl.Value.HasValue, Is.EqualTo(expectedResult.HasValue),"Future double min has failed");
				if (expectedResult.HasValue)
					Assert.That(futureDbl.Value.Value, Is.EqualTo(expectedResult).Within(0.001d), "Future double min has failed");

				var date = dcQuery.Min(dc => dc.DateTime);
				var dateWithOffset = dcQuery.Min(dc => dc.DateTimeOffset);
				var futureDate = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.DateTime));
				var futureDateWithOffset = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.DateTimeOffset));
				if (expectedResult.HasValue)
				{
					Assert.That(date, Is.LessThan(_testDate), "DateTime min has failed");
					Assert.That(dateWithOffset, Is.LessThan(_testDateWithOffset), "DateTimeOffset min has failed");
					Assert.That(futureDate.Value, Is.LessThan(_testDate), "Future DateTime min has failed");
					Assert.That(futureDateWithOffset.Value, Is.LessThan(_testDateWithOffset), "Future DateTimeOffset min has failed");
				}
				else
				{
					Assert.That(date, Is.Null, "DateTime min has failed");
					Assert.That(dateWithOffset, Is.Null, "DateTimeOffset min has failed");
					Assert.That(futureDate.Value, Is.Null, "Future DateTime min has failed");
					Assert.That(futureDateWithOffset.Value, Is.Null, "Future DateTimeOffset min has failed");
				}

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.That(() => nonNullableDecimal = dcQuery.Min(dc => dc.NonNullableDecimal), Throws.Nothing, "Non nullable decimal min has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Non nullable decimal min has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.NonNullableDecimal));
					Assert.That(() => nonNullableDecimal = futureNonNullableDec.Value, Throws.Nothing, "Future non nullable decimal min has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Future non nullable decimal min has failed");
				}
				else
				{
					Assert.That(() => dcQuery.Min(dc => dc.NonNullableDecimal),
					            // After fix
					            Throws.InstanceOf<InvalidOperationException>()
					                  // Before fix
					                  .Or.InnerException.InstanceOf<ArgumentNullException>(),
					            "Non nullable decimal min has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.NonNullableDecimal));
					Assert.That(() => futureNonNullableDec.Value,
					            Throws.TargetInvocationException.And.InnerException.InstanceOf<InvalidOperationException>(),
					            "Future non nullable decimal min has failed");
				}
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultBBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassBExtendedByA>();
				Assert.That(() => query.SingleOrDefault(), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassBExtendedByA>();
				DomainClassBExtendedByA result = null;
				Assert.That(() => result = query.SingleOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>());
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
				Assert.That(result, Is.TypeOf<DomainClassBExtendedByA>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultCBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassCExtendedByD>();
				Assert.That(() => query.SingleOrDefault(), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassCExtendedByD>();
				DomainClassCExtendedByD result = null;
				Assert.That(() => result = query.SingleOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>());
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
				Assert.That(result, Is.TypeOf<DomainClassCExtendedByD>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultE()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassE>();
				Assert.That(() => query.SingleOrDefault(), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultEWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassE>();
				DomainClassE result = null;
				Assert.That(() => result = query.SingleOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Name, Is.EqualTo(_searchName1));
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Not.Null, "Future");
				Assert.That(result.Name, Is.EqualTo(_searchName1), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultF()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassF>();
				DomainClassF result = null;
				Assert.That(() => result = query.SingleOrDefault(), Throws.Nothing);
				Assert.That(result, Is.Null);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Null, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultFWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassF>();
				DomainClassF result = null;
				Assert.That(() => result = query.SingleOrDefault(dc => dc.Name == _searchName1), Throws.Nothing);
				Assert.That(result, Is.Null);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => result = futureQuery.Value, Throws.Nothing, "Future");
				Assert.That(result, Is.Null, "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultGBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				Assert.That(() => query.SingleOrDefault(), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				Assert.That(() => query.SingleOrDefault(dc => dc.Name == _searchName1), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault(dc => dc.Name == _searchName1));
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultObject()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<object>();
				Assert.That(() => query.SingleOrDefault(), Throws.InvalidOperationException);
				var futureQuery = query.ToFutureValue(qdc => qdc.SingleOrDefault());
				Assert.That(() => futureQuery.Value, Throws.TargetInvocationException.And.InnerException.TypeOf<InvalidOperationException>(), "Future");
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void SumBBase()
		{
			Sum<DomainClassBExtendedByA>(3);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void SumCBase()
		{
			Sum<DomainClassCExtendedByD>(3);
		}

		// Non-reg case
		[Test]
		public void SumE()
		{
			Sum<DomainClassE>(3);
		}

		// Non-reg case
		[Test]
		public void SumF()
		{
			Sum<DomainClassF>(null);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void SumGBase()
		{
			Sum<DomainClassGExtendedByH>(10);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void SumObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Sum(o => (int?)2);
				Assert.That(result, Is.EqualTo(_totalEntityCount * 2));
			}
		}

		private void Sum<DC>(int? expectedResult) where DC : DomainClassBase
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var integ = dcQuery.Sum(dc => dc.Integer);
				Assert.That(integ, Is.EqualTo(expectedResult), "Integer sum has failed");
				var futureInteg = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.Integer));
				Assert.That(futureInteg.Value, Is.EqualTo(expectedResult), "Future integer sum has failed");

				var longInt = dcQuery.Sum(dc => dc.Long);
				Assert.That(longInt, Is.EqualTo(expectedResult), "Long integer sum has failed");
				var futureLongInt = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.Long));
				Assert.That(futureLongInt.Value, Is.EqualTo(expectedResult), "Future long integer sum has failed");

				var dec = dcQuery.Sum(dc => dc.Decimal);
				Assert.That(dec, Is.EqualTo(expectedResult), "Decimal sum has failed");
				var futureDec = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.Decimal));
				Assert.That(futureDec.Value, Is.EqualTo(expectedResult), "Future decimal sum has failed");

				var dbl = dcQuery.Sum(dc => dc.Double);
				Assert.That(dbl.HasValue, Is.EqualTo(expectedResult.HasValue), "Double sum has failed");
				if (expectedResult.HasValue)
					Assert.That(dbl.Value, Is.EqualTo(expectedResult).Within(0.001d), "Double sum has failed");
				var futureDbl = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.Double));
				Assert.That(futureDbl.Value.HasValue, Is.EqualTo(expectedResult.HasValue), "Future double sum has failed");
				if (expectedResult.HasValue)
					Assert.That(futureDbl.Value.Value, Is.EqualTo(expectedResult).Within(0.001d), "Future double sum has failed");

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.That(() => nonNullableDecimal = dcQuery.Sum(dc => dc.NonNullableDecimal), Throws.Nothing, "Non nullable decimal sum has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Non nullable decimal sum has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.NonNullableDecimal));
					Assert.That(() => nonNullableDecimal = futureNonNullableDec.Value, Throws.Nothing, "Future non nullable decimal sum has failed");
					Assert.That(nonNullableDecimal, Is.EqualTo(expectedResult), "Future non nullable decimal sum has failed");
				}
				else
				{
					Assert.That(() => dcQuery.Sum(dc => dc.NonNullableDecimal),
					            // After fix
					            Throws.InstanceOf<InvalidOperationException>()
					                  // Before fix
					                  .Or.InnerException.InstanceOf<ArgumentNullException>(),
					            "Non nullable decimal sum has failed");
					var futureNonNullableDec = dcQuery.ToFutureValue(qdc => qdc.Sum(dc => dc.NonNullableDecimal));
					Assert.That(() => futureNonNullableDec.Value,
					            Throws.TargetInvocationException.And.InnerException.InstanceOf<InvalidOperationException>(),
					            "Future non nullable decimal sum has failed");
				}
			}
		}

		[Test]
		public void BadOverload()
		{
			var sumMethodTemplate = ReflectHelper.GetMethod(() => Queryable.Sum((IQueryable<int>)null));
			Assert.Throws<InvalidOperationException>(() =>
			{
				ReflectHelper.GetMethodOverload(sumMethodTemplate, typeof(object));
			});
		}
		
		[Test, Explicit("Just a blunt perf comparison among some candidate overload reflection patterns, one being required for NH-3850")]
		public void OverloadReflectionBluntPerfCompare()
		{
			var sumMethodTemplate = ReflectHelper.GetMethod(() => Queryable.Sum((IQueryable<int>)null));

			var swNoSameParamsCheck = new Stopwatch();
			swNoSameParamsCheck.Start();
			for (var i = 0; i < 1000; i++)
			{
				var sumMethod = sumMethodTemplate.DeclaringType.GetMethod(sumMethodTemplate.Name,
					(sumMethodTemplate.IsStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public,
					null, new[] { typeof(IQueryable<decimal>) }, null);
				Trace.TraceInformation(sumMethod.ToString());
			}
			swNoSameParamsCheck.Stop();
			
			var swCurrentChoiceSameType = new Stopwatch();
			swCurrentChoiceSameType.Start();
			for (var i = 0; i < 1000; i++)
			{
				var sumMethod = ReflectHelper.GetMethodOverload(sumMethodTemplate, typeof(IQueryable<int>));
				Trace.TraceInformation(sumMethod.ToString());
			}
			swCurrentChoiceSameType.Stop();

			var swCurrentChoice = new Stopwatch();
			swCurrentChoice.Start();
			for (var i = 0; i < 1000; i++)
			{
				var sumMethod = ReflectHelper.GetMethodOverload(sumMethodTemplate, typeof(IQueryable<long>));
				Trace.TraceInformation(sumMethod.ToString());
			}
			swCurrentChoice.Stop();

			var swEnHlp = new Stopwatch();
			swEnHlp.Start();
			for (var i = 0; i < 1000; i++)
			{
				// Testing the obsolete helper perf. Disable obsolete warning. Remove this swEnHlp part of the test if this helper is to be removed.
#pragma warning disable 0618
				var sumMethod = EnumerableHelper.GetMethod("Sum", new[] { typeof(IEnumerable<int>) });
#pragma warning restore 0618
				Trace.TraceInformation(sumMethod.ToString());
			}
			swEnHlp.Stop();

			Assert.Pass(@"Blunt perf timings:
Direct reflection: {0}
Current impl, same overload: {1}
Current impl, other overload: {2}
EnumerableHelper.GetMethod(non generic overload): {3}",
				swNoSameParamsCheck.Elapsed, swCurrentChoiceSameType.Elapsed, swCurrentChoice.Elapsed, swEnHlp.Elapsed);
		}
	}
}
