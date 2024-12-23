﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.SqlTypes;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using NHibernate.Type;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class DateTimeTestsAsync : TestCase
	{
		private bool DialectSupportsDateTimeOffset => TestDialect.SupportsSqlType(new SqlType(DbType.DateTimeOffset));
		private bool DialectSupportsDateTimeWithScale => TestDialect.SupportsSqlType(new SqlType(DbType.DateTime,(byte) 2));
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
				entity.DateTimeValueWithScale = entity.DateTimeValue.AddSeconds(0.9);
				entity.DateTimeOffsetValue = new DateTimeOffset(entity.DateTimeValue, TimeSpan.FromHours(3));
				entity.DateTimeOffsetValueWithScale = new DateTimeOffset(entity.DateTimeValue, TimeSpan.FromHours(3));
			}

			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				foreach (var entity in _referenceEntities)
				{
					session.Save(entity);
				}
				trans.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				session.Query<DateTimeTestsClass>().Delete();
				trans.Commit();
			}
		}

		private void AssertDateTimeOffsetSupported()
		{
			if (!DialectSupportsDateTimeOffset)
			{
				Assert.Ignore("Dialect doesn't support DateTimeOffset");
			}
		}

		private async Task AssertDateTimeWithScaleSupportedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!DialectSupportsDateTimeWithScale)
			{
				Assert.Ignore("Dialect doesn't support DateTime with scale (2)");
			}
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entity1 = await (session.GetAsync<DateTimeTestsClass>(_referenceEntities[0].Id, cancellationToken));
				if (entity1.DateTimeValueWithScale != entity1.DateTimeValue.AddSeconds(0.9))
				{
					Assert.Ignore("Current setup doesn't support DateTime with scale (2)");
				}
			}

		}

		private Task AssertQueryAsync(Expression<Func<DateTimeTestsClass, bool>> where, CancellationToken cancellationToken = default(CancellationToken)) => AssertQueryAsync(where, x => x.Id, cancellationToken);

		private async Task AssertQueryAsync<TSelect>(Expression<Func<DateTimeTestsClass, bool>> where, Expression<Func<DateTimeTestsClass, TSelect>> select, CancellationToken cancellationToken = default(CancellationToken))
		{
			using var session = OpenSession();
			var fromDb = await (session.Query<DateTimeTestsClass>().Where(where).Select(select).ToListAsync(cancellationToken));
			var fromMemory = _referenceEntities.AsQueryable().Where(where).Select(select).AsEnumerable().ToList(); //AsEnumerable added to avoid async generator
			Assert.That(fromMemory, Has.Count.GreaterThan(0), "Inconclusive, since the query doesn't match anything in the defined set");
			Assert.That(fromDb, Has.Count.EqualTo(fromMemory.Count));
			Assert.That(fromDb, Is.EquivalentTo(fromMemory));
		}

		[Test]
		public async Task CanQueryByYearAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Year == 1998));
		}

		[Test]
		public async Task CanQueryDateTimeBySecondAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Second == 4));
		}

		[Test]
		public async Task CanQueryDateTimeByMinuteAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Minute == 4));
		}

		[Test]
		public async Task CanQueryDateTimeByHourAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Hour == 4));
		}

		[Test]
		public async Task CanQueryDateTimeBySecondWhenValueContainsFractionalSecondsAsync()
		{
			await (AssertDateTimeWithScaleSupportedAsync());
			await (AssertQueryAsync(o => o.DateTimeValueWithScale.Second == 4));
		}

		[Test]
		public async Task CanQueryDateTimeOffsetBySecondAsync()
		{
			AssertDateTimeOffsetSupported();
			await (AssertQueryAsync(o => o.DateTimeOffsetValue.Second == 4));
		}

		[Test]
		public async Task CanQueryDateTimeOffsetByMinuteAsync()
		{
			AssertDateTimeOffsetSupported();
			await (AssertQueryAsync(o => o.DateTimeOffsetValue.Minute == 4));
		}

		[Test]
		public async Task CanQueryDateTimeOffsetByHourAsync()
		{
			AssertDateTimeOffsetSupported();
			await (AssertQueryAsync(o => o.DateTimeOffsetValue.Hour == 4));
		}

		[Test]
		public async Task CanQueryDateTimeOffsetBySecondWhenValueContainsFractionalSecondsAsync()
		{
			AssertDateTimeOffsetSupported();
			await (AssertQueryAsync(o => o.DateTimeOffsetValueWithScale.Second == 4));
		}

		[Test]
		public async Task CanQueryByDateAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Date == new DateTime(1998, 02, 26)));
		}

		[Test]
		public async Task CanQueryByDateTimeAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue == new DateTime(1998, 02, 26)));
		}

		[Test]
		public async Task CanQueryByDateTime2Async()
		{
			await (AssertQueryAsync(o => o.DateTimeValue == new DateTime(1998, 02, 26, 1, 1, 1)));
		}

		[Test]
		public async Task CanSelectYearAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Year == 1998, o => o.DateTimeValue.Year));
		}

		[Test]
		public async Task CanSelectDateAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue.Date == new DateTime(1998, 02, 26), o => o.DateTimeValue.Date));
		}

		[Test]
		public async Task CanSelectDateTimeAsync()
		{
			await (AssertQueryAsync(o => o.DateTimeValue == new DateTime(1998, 02, 26), o => o.DateTimeValue));
		}

		[Test]
		public async Task CanSelectDateTime2Async()
		{
			await (AssertQueryAsync(o => o.DateTimeValue == new DateTime(1998, 02, 26, 1, 1, 1), o => o.DateTimeValue));
		}

		[Test]
		public async Task CanSelectDateTimeWithScaleAsync()
		{
			await (AssertDateTimeWithScaleSupportedAsync());
			await (AssertQueryAsync(o => o.DateTimeValueWithScale == _referenceEntities[0].DateTimeValueWithScale, o => o.DateTimeValueWithScale));
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
