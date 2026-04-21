using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class DateTimeTests : TestCase
	{
		private bool DialectSupportsDateTimeOffset => TestDialect.SupportsSqlType(new SqlType(DbType.DateTimeOffset));
		private readonly DateTimeTestsClass[] _referenceEntities =
		[
			new() {Id =1, DateTimeValue = new DateTime(1998, 02, 26)},
			new() {Id =2, DateTimeValue = new DateTime(1998, 02, 26)},
			new() {Id =3, DateTimeValue = new DateTime(1998, 02, 26, 01, 01, 01)},
			new() {Id =4, DateTimeValue = new DateTime(1998, 02, 26, 02, 02, 02)},
			new() {Id =5, DateTimeValue = new DateTime(1998, 02, 26, 03, 03, 03)},
			new() {Id =6, DateTimeValue = new DateTime(1998, 02, 26, 04, 04, 04)},
			new() {Id =7, DateTimeValue = new DateTime(1998, 03, 01)},
			new() {Id =8, DateTimeValue = new DateTime(2000, 01, 01)}
		];

		private TimeSpan FractionalSecondsAdded => TimeSpan.FromMilliseconds(900);

		protected override string[] Mappings => default;
		protected override void AddMappings(Configuration configuration)
		{
			var modelMapper = new ModelMapper();

			modelMapper.Class<DateTimeTestsClass>(m =>
			{
				m.Table("datetimetests");
				m.Lazy(false);
				m.Id(p => p.Id, p => p.Generator(Generators.Assigned));
				m.Property(p => p.DateValue, c => c.Type<DateType>());
				m.Property(p => p.DateTimeValue);
				m.Property(p => p.DateTimeValueWithScale, c => c.Scale(2));
				if (DialectSupportsDateTimeOffset)
				{
					m.Property(p => p.DateTimeOffsetValue);
					m.Property(p => p.DateTimeOffsetValueWithScale, c => c.Scale(2));
				}
			});
			var mapping = modelMapper.CompileMappingForAllExplicitlyAddedEntities();
			configuration.AddMapping(mapping);
		}

		protected override void OnSetUp()
		{
			foreach (var entity in _referenceEntities)
			{
				entity.DateValue = entity.DateTimeValue.Date;
				entity.DateTimeValueWithScale = entity.DateTimeValue + FractionalSecondsAdded;
				entity.DateTimeOffsetValue = new DateTimeOffset(entity.DateTimeValue, TimeSpan.FromHours(3));
				entity.DateTimeOffsetValueWithScale = new DateTimeOffset(entity.DateTimeValueWithScale, TimeSpan.FromHours(3));
			}

			using var session = OpenSession();
			using var trans = session.BeginTransaction();
			foreach (var entity in _referenceEntities)
			{
				session.Save(entity);
			}
			trans.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var trans = session.BeginTransaction();
			session.Query<DateTimeTestsClass>().Delete();
			trans.Commit();
		}

		private void AssertDateTimeOffsetSupported()
		{
			if (!DialectSupportsDateTimeOffset)
			{
				Assert.Ignore("Dialect doesn't support DateTimeOffset");
			}
		}

		private void AssertDateTimeWithFractionalSecondsSupported()
		{
			//Ideally, the dialect should know whether this is supported or not
			if (!TestDialect.SupportsDateTimeWithFractionalSeconds)
			{
				Assert.Ignore("Dialect doesn't support DateTime with factional seconds");
			}

			//But it sometimes doesn't
			using var session = OpenSession();
			using var trans = session.BeginTransaction();
			var entity1 = session.Get<DateTimeTestsClass>(_referenceEntities[0].Id);
			trans.Commit();
			if (entity1.DateTimeValueWithScale != entity1.DateTimeValue + FractionalSecondsAdded)
			{
				Assert.Ignore("Current setup doesn't support DateTime with scale (2)");
			}
		}

		private void AssertQuery(Expression<Func<DateTimeTestsClass, bool>> where) => AssertQuery(where, x => x.Id);

		private void AssertQuery<TSelect>(Expression<Func<DateTimeTestsClass, bool>> where, Expression<Func<DateTimeTestsClass, TSelect>> select)
		{
			using var session = OpenSession();
			var fromDb = session.Query<DateTimeTestsClass>().Where(where).Select(select).ToList();
			var fromMemory = _referenceEntities.AsQueryable().Where(where).Select(select).AsEnumerable().ToList(); //AsEnumerable added to avoid async generator
			Assert.That(fromMemory, Has.Count.GreaterThan(0), "Inconclusive, since the query doesn't match anything in the defined set");
			Assert.That(fromDb, Has.Count.EqualTo(fromMemory.Count));
			Assert.That(fromDb, Is.EquivalentTo(fromMemory));
		}

		[Test]
		public void CanQueryByYear()
		{
			AssertQuery(o => o.DateTimeValue.Year == 1998);
		}

		[Test]
		public void CanQueryDateTimeBySecond()
		{
			AssertQuery(o => o.DateTimeValue.Second == 4);
		}

		[Test]
		public void CanQueryDateTimeByMinute()
		{
			AssertQuery(o => o.DateTimeValue.Minute == 4);
		}

		[Test]
		public void CanQueryDateTimeByHour()
		{
			AssertQuery(o => o.DateTimeValue.Hour == 4);
		}

		[Test]
		public void CanQueryDateTimeBySecondWhenValueContainsFractionalSeconds()
		{
			AssertDateTimeWithFractionalSecondsSupported();
			AssertQuery(o => o.DateTimeValueWithScale.Second == 4);
		}

		[Test]
		public void CanQueryDateTimeOffsetBySecond()
		{
			AssertDateTimeOffsetSupported();
			AssertQuery(o => o.DateTimeOffsetValue.Second == 4);
		}

		[Test]
		public void CanQueryDateTimeOffsetByMinute()
		{
			AssertDateTimeOffsetSupported();
			AssertQuery(o => o.DateTimeOffsetValue.Minute == 4);
		}

		[Test]
		public void CanQueryDateTimeOffsetByHour()
		{
			AssertDateTimeOffsetSupported();
			AssertQuery(o => o.DateTimeOffsetValue.Hour == 4);
		}

		[Test]
		public void CanQueryDateTimeOffsetBySecondWhenValueContainsFractionalSeconds()
		{
			AssertDateTimeOffsetSupported();
			AssertQuery(o => o.DateTimeOffsetValueWithScale.Second == 4);
		}

		[Test]
		public void CanQueryByDate()
		{
			AssertQuery(o => o.DateTimeValue.Date == new DateTime(1998, 02, 26));
		}

		[Test]
		public void CanQueryByDateTime()
		{
			AssertQuery(o => o.DateTimeValue == new DateTime(1998, 02, 26));
		}

		[Test]
		public void CanQueryByDateTime2()
		{
			AssertQuery(o => o.DateTimeValue == new DateTime(1998, 02, 26, 1, 1, 1));
		}

		[Test]
		public void CanSelectYear()
		{
			AssertQuery(o => o.DateTimeValue.Year == 1998, o => o.DateTimeValue.Year);
		}

		[Test]
		public void CanSelectDate()
		{
			AssertQuery(o => o.DateTimeValue.Date == new DateTime(1998, 02, 26), o => o.DateTimeValue.Date);
		}

		[Test]
		public void CanSelectDateTime()
		{
			AssertQuery(o => o.DateTimeValue == new DateTime(1998, 02, 26), o => o.DateTimeValue);
		}

		[Test]
		public void CanSelectDateTime2()
		{
			AssertQuery(o => o.DateTimeValue == new DateTime(1998, 02, 26, 1, 1, 1), o => o.DateTimeValue);
		}

		[Test]
		public void CanSelectDateTimeWithScale()
		{
			AssertDateTimeWithFractionalSecondsSupported();
			AssertQuery(o => o.DateTimeValueWithScale == _referenceEntities[0].DateTimeValueWithScale, o => o.DateTimeValueWithScale);
		}

		public class DateTimeTestsClass : IEquatable<DateTimeTestsClass>
		{
			public int Id { get; set; }
			public DateTime DateTimeValue { get; set; }
			public DateTime DateTimeValueWithScale { get; set; }
			public DateTimeOffset DateTimeOffsetValue { get; set; }
			public DateTimeOffset DateTimeOffsetValueWithScale { get; set; }
			public DateTime DateValue { get; set; }

			public override bool Equals(object obj)
			{
				return Equals(obj as DateTimeTestsClass);
			}

			public bool Equals(DateTimeTestsClass other)
			{
				return other is not null &&
					   Id.Equals(other.Id);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Id);
			}
		}
	}
}
