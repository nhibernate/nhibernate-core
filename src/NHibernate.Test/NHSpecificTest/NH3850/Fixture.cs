using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Linq;
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
			var typeNames = (TypeNames)typeof(Dialect.Dialect).GetField("_typeNames", ReflectHelper.AnyVisibilityInstance).GetValue(Dialect);
			try
			{
				typeNames.Get(DbType.DateTimeOffset);
			}
			catch (ArgumentException)
			{
				return false;
			}

			return true;
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			// Cannot handle DbType.DateTimeOffset via ODBC.
			return !(factory.ConnectionProvider.Driver is OdbcDriver);
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
				var result = session.Query<DomainClassGExtendedByH>()
					.OrderBy(dc => dc.Id)
					.Select(dc => dc.Id)
					.Aggregate((p, n) => p + n);
				Assert.AreEqual(10, result);
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
				var result = session.Query<DomainClassGExtendedByH>()
					.OrderByDescending(dc => dc.Id)
					.Select(dc => dc.Id.ToString())
					.Aggregate((p, n) => p + "," + n);
				// Currently yields "2,1,4,3" instead.
				Assert.AreEqual("4,3,2,1", result);
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
				var result = session.Query<DomainClassGExtendedByH>()
					.OrderBy(dc => dc.Id)
					.Aggregate(new StringBuilder(), (s, dc) => s.Append(dc.Name).Append(","));
				Assert.AreEqual(_searchName1 + "," + _searchName2 + "," + _searchName1 + "," + _searchName2 + ",", result.ToString());
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AggregateSeedGBase()
		{
			using (var session = OpenSession())
			{
				// This case should work because the aggregate is insensitive to ordering.
				var result = session.Query<DomainClassGExtendedByH>()
					.OrderBy(dc => dc.Id)
					.Aggregate(5, (s, dc) => s + dc.Id);
				Assert.AreEqual(15, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AllBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().All(dc => dc.Name == _searchName1);
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AllCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().All(dc => dc.Name == _searchName1);
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AllEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().All(dc => dc.Name == _searchName1);
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AllFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().All(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AllGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().All(dc => dc.Name == _searchName1);
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AllGBaseWithNameFilteredByName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>()
					.Where(dc => dc.Name == _searchName1)
					.All(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Any();
				Assert.IsTrue(result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Any(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Any();
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Any(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Any();
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Any(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Any();
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Any(dc => dc.Name == _searchName1);
				Assert.IsFalse(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Any();
				Assert.IsTrue(result);
			}
		}

		// Non-reg case
		[Test]
		public void AnyGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Any(dc => dc.Name == _searchName1);
				Assert.IsTrue(result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void AnyObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Any();
				Assert.IsTrue(result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageBBase()
		{
			Average<DomainClassBExtendedByA>(1.5m);
		}

		// Failing case till NH-3850 is fixed
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

		// Failing case till NH-3850 is fixed
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
				var longInt = dcQuery.Average(dc => dc.Long);
				Assert.AreEqual(expectedResult, longInt, "Long integer average has failed");
				var dec = dcQuery.Average(dc => dc.Decimal);
				Assert.AreEqual(expectedResult, dec, "Decimal average has failed");
				var dbl = dcQuery.Average(dc => dc.Double);
				Assert.AreEqual(expectedResult, dbl, "Double average has failed");

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.DoesNotThrow(() => { nonNullableDecimal = dcQuery.Average(dc => dc.NonNullableDecimal); }, "Non nullable decimal average has failed");
					Assert.AreEqual(expectedResult, nonNullableDecimal, "Non nullable decimal average has failed");
				}
				else
				{
					Assert.That(() => { dcQuery.Average(dc => dc.NonNullableDecimal); },
						// After fix
						Throws.InstanceOf<InvalidOperationException>()
						// Before fix
						.Or.InnerException.InstanceOf<ArgumentNullException>(),
						"Non nullable decimal average has failed");
				}
			}
		}

		// Failing case till NH-3850 is fixed
		[Test, Ignore("Won't fix: requires reshaping the query")]
		public void AverageObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Average(o => (int?)2);
				Assert.AreEqual(2, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Count();
				Assert.AreEqual(2, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().Count(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Count();
				Assert.AreEqual(2, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().Count(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Count();
				Assert.AreEqual(2, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().Count(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Count();
				Assert.AreEqual(0, result);
			}
		}

		// Non-reg case
		[Test]
		public void CountFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().Count(dc => dc.Name == _searchName1);
				Assert.AreEqual(0, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Count();
				Assert.AreEqual(4, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().Count(dc => dc.Name == _searchName1);
				Assert.AreEqual(2, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void CountObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().Count();
				Assert.AreEqual(_totalEntityCount, result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNotNull(result);
				Assert.IsInstanceOf<DomainClassBExtendedByA>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassBExtendedByA>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNotNull(result);
				Assert.IsInstanceOf<DomainClassCExtendedByD>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassCExtendedByD>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNotNull(result);
				Assert.IsInstanceOf<DomainClassE>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassE>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNull(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNull(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNotNull(result);
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.IsInstanceOf<DomainClassGExtendedByH>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.IsInstanceOf<DomainClassGExtendedByH>(result);
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
				Assert.DoesNotThrow(() => { result = query.FirstOrDefault(); });
				Assert.IsNotNull(result);
				// If class type assert starts failing, maybe just ignore it: order of first on polymorphic queries looks unspecified to me.
				Assert.IsInstanceOf<DomainClassBExtendedByA>(result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountBBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().LongCount();
				Assert.AreEqual(2, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountBBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassBExtendedByA>().LongCount(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountCBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().LongCount();
				Assert.AreEqual(2, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountCBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassCExtendedByD>().LongCount(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountE()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().LongCount();
				Assert.AreEqual(2, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountEWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassE>().LongCount(dc => dc.Name == _searchName1);
				Assert.AreEqual(1, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountF()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().LongCount();
				Assert.AreEqual(0, result);
			}
		}

		// Non-reg case
		[Test]
		public void LongCountFWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassF>().LongCount(dc => dc.Name == _searchName1);
				Assert.AreEqual(0, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountGBase()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().LongCount();
				Assert.AreEqual(4, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<DomainClassGExtendedByH>().LongCount(dc => dc.Name == _searchName1);
				Assert.AreEqual(2, result);
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void LongCountObject()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<object>().LongCount();
				Assert.AreEqual(_totalEntityCount, result);
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
				Assert.AreEqual(expectedResult.HasValue ? _searchName2 : null, name, "String max has failed");
				var integ = dcQuery.Max(dc => dc.Integer);
				Assert.AreEqual(expectedResult, integ, "Integer max has failed");
				var longInt = dcQuery.Max(dc => dc.Long);
				Assert.AreEqual(expectedResult, longInt, "Long integer max has failed");
				var dec = dcQuery.Max(dc => dc.Decimal);
				Assert.AreEqual(expectedResult, dec, "Decimal max has failed");
				var dbl = dcQuery.Max(dc => dc.Double);
				Assert.AreEqual(expectedResult.HasValue, dbl.HasValue, "Double max has failed");
				if (expectedResult.HasValue)
					Assert.AreEqual(expectedResult.Value, dbl.Value, 0.001d, "Double max has failed");

				var date = dcQuery.Max(dc => dc.DateTime);
				var dateWithOffset = dcQuery.Max(dc => dc.DateTimeOffset);
				if (expectedResult.HasValue)
				{
					Assert.Greater(date, _testDate, "DateTime max has failed");
					Assert.Greater(dateWithOffset, _testDateWithOffset, "DateTimeOffset max has failed");
				}
				else
				{
					Assert.Null(date, "DateTime max has failed");
					Assert.Null(dateWithOffset, "DateTimeOffset max has failed");
				}

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.DoesNotThrow(() => { nonNullableDecimal = dcQuery.Max(dc => dc.NonNullableDecimal); }, "Non nullable decimal max has failed");
					Assert.AreEqual(expectedResult, nonNullableDecimal, "Non nullable decimal max has failed");
				}
				else
				{
					Assert.That(() => { dcQuery.Max(dc => dc.NonNullableDecimal); },
						// After fix
						Throws.InstanceOf<InvalidOperationException>()
						// Before fix
						.Or.InnerException.InstanceOf<ArgumentNullException>(),
						"Non nullable decimal max has failed");
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
				Assert.AreEqual(expectedResult.HasValue ? _searchName1 : null, name, "String min has failed");
				var integ = dcQuery.Min(dc => dc.Integer);
				Assert.AreEqual(expectedResult, integ, "Integer min has failed");
				var longInt = dcQuery.Min(dc => dc.Long);
				Assert.AreEqual(expectedResult, longInt, "Long integer min has failed");
				var dec = dcQuery.Min(dc => dc.Decimal);
				Assert.AreEqual(expectedResult, dec, "Decimal min has failed");
				var dbl = dcQuery.Min(dc => dc.Double);
				Assert.AreEqual(expectedResult.HasValue, dbl.HasValue, "Double min has failed");
				if (expectedResult.HasValue)
					Assert.AreEqual(expectedResult.Value, dbl.Value, 0.001d, "Double min has failed");

				var date = dcQuery.Min(dc => dc.DateTime);
				var dateWithOffset = dcQuery.Min(dc => dc.DateTimeOffset);
				if (expectedResult.HasValue)
				{
					Assert.Less(date, _testDate, "DateTime min has failed");
					Assert.Less(dateWithOffset, _testDateWithOffset, "DateTimeOffset min has failed");
				}
				else
				{
					Assert.Null(date, "DateTime min has failed");
					Assert.Null(dateWithOffset, "DateTimeOffset min has failed");
				}

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.DoesNotThrow(() => { nonNullableDecimal = dcQuery.Min(dc => dc.NonNullableDecimal); }, "Non nullable decimal min has failed");
					Assert.AreEqual(expectedResult, nonNullableDecimal, "Non nullable decimal min has failed");
				}
				else
				{
					Assert.That(() => { dcQuery.Min(dc => dc.NonNullableDecimal); },
						// After fix
						Throws.InstanceOf<InvalidOperationException>()
						// Before fix
						.Or.InnerException.InstanceOf<ArgumentNullException>(),
						"Non nullable decimal min has failed");
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
				DomainClassBExtendedByA result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(); });
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
				Assert.DoesNotThrow(() => { result = query.SingleOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassBExtendedByA>(result);
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultCBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassCExtendedByD>();
				DomainClassCExtendedByD result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(); });
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
				Assert.DoesNotThrow(() => { result = query.SingleOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassCExtendedByD>(result);
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultE()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassE>();
				DomainClassE result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(); });
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
				Assert.DoesNotThrow(() => { result = query.SingleOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNotNull(result);
				Assert.AreEqual(_searchName1, result.Name);
				Assert.IsInstanceOf<DomainClassE>(result);
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
				Assert.DoesNotThrow(() => { result = query.SingleOrDefault(); });
				Assert.IsNull(result);
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
				Assert.DoesNotThrow(() => { result = query.SingleOrDefault(dc => dc.Name == _searchName1); });
				Assert.IsNull(result);
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultGBase()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				DomainClassGExtendedByH result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(); });
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultGBaseWithName()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<DomainClassGExtendedByH>();
				DomainClassGExtendedByH result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(dc => dc.Name == _searchName1); });
			}
		}

		// Non-reg case
		[Test]
		public void SingleOrDefaultObject()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<object>();
				object result = null;
				Assert.Throws<InvalidOperationException>(() => { result = query.SingleOrDefault(); });
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
				Assert.AreEqual(_totalEntityCount * 2, result);
			}
		}

		private void Sum<DC>(int? expectedResult) where DC : DomainClassBase
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var integ = dcQuery.Sum(dc => dc.Integer);
				Assert.AreEqual(expectedResult, integ, "Integer sum has failed");
				var longInt = dcQuery.Sum(dc => dc.Long);
				Assert.AreEqual(expectedResult, longInt, "Long integer sum has failed");
				var dec = dcQuery.Sum(dc => dc.Decimal);
				Assert.AreEqual(expectedResult, dec, "Decimal sum has failed");
				var dbl = dcQuery.Sum(dc => dc.Double);
				Assert.AreEqual(expectedResult.HasValue, dbl.HasValue, "Double sum has failed");
				if (expectedResult.HasValue)
					Assert.AreEqual(expectedResult.Value, dbl.Value, 0.001d, "Double sum has failed");

				if (expectedResult.HasValue)
				{
					var nonNullableDecimal = -1m;
					Assert.DoesNotThrow(() => { nonNullableDecimal = dcQuery.Sum(dc => dc.NonNullableDecimal); }, "Non nullable decimal sum has failed");
					Assert.AreEqual(expectedResult, nonNullableDecimal, "Non nullable decimal sum has failed");
				}
				else
				{
					Assert.That(() => { dcQuery.Sum(dc => dc.NonNullableDecimal); },
						// After fix
						Throws.InstanceOf<InvalidOperationException>()
						// Before fix
						.Or.InnerException.InstanceOf<ArgumentNullException>(),
						"Non nullable decimal sum has failed");
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