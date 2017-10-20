using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class TimeFixture : TypeFixtureBase
	{
		protected override string TypeName => "Time";

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from TimeClass").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void PropertiesHasExpectedType()
		{
			var classMetaData = Sfi.GetClassMetadata(typeof(TimeClass));
			Assert.That(
				classMetaData.GetPropertyType(nameof(TimeClass.TimeValue)),
				Is.EqualTo(NHibernateUtil.Time));
			Assert.That(
				classMetaData.GetPropertyType(nameof(TimeClass.TimeWithScale)),
				Is.EqualTo(TypeFactory.GetTimeType(3)));
		}

		[Test]
		public void SavingAndRetrieving()
		{
			var ticks = DateTime.Parse("23:59:59");

			var entity =
				new TimeClass
				{
					TimeValue = ticks
				};

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var entityReturned = s.CreateQuery("from TimeClass").UniqueResult<TimeClass>();

				Assert.That(entityReturned.TimeValue.TimeOfDay, Is.EqualTo(ticks.TimeOfDay));
				Assert.That(ticks.Hour, Is.EqualTo(entityReturned.TimeValue.Hour));
				Assert.That(ticks.Minute, Is.EqualTo(entityReturned.TimeValue.Minute));
				Assert.That(ticks.Second, Is.EqualTo(entityReturned.TimeValue.Second));
				tx.Commit();
			}
		}

		[Test]
		public void LowerDigitsAreIgnored()
		{
			if (!Dialect.SupportsDateTimeScale)
				Assert.Ignore("Lower digits cannot be ignored when dialect does not support scale");

			var baseTime = new DateTime(1990, 01, 01, 17, 55, 24, 548);
			var entity = new TimeClass
			{
				TimeWithScale = baseTime.Add(TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond / 3))
			};
			Assert.That(entity.TimeWithScale, Is.Not.EqualTo(baseTime));

			int id;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(entity);
				id = entity.Id;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var retrieved = s.Load<TimeClass>(id);
				Assert.That(retrieved.TimeWithScale.TimeOfDay, Is.EqualTo(baseTime.TimeOfDay));
				t.Commit();
			}
		}
	}
}
