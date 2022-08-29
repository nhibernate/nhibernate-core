using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for TimeAsTimeSpanTypeFixture.
	/// </summary>
	[TestFixture]
	public class TimeAsTimeSpanTypeFixture
	{
		[Test]
		public void Next()
		{
			var type = (TimeAsTimeSpanType) NHibernateUtil.TimeAsTimeSpan;
			object current = new TimeSpan(DateTime.Now.Ticks - 5);
			object next = type.Next(current, null);

			Assert.IsTrue(next is TimeSpan, "Next should be TimeSpan");
			Assert.IsTrue((TimeSpan) next > (TimeSpan) current,
						  "next should be greater than current (could be equal depending on how quickly this occurs)");
		}

		[Test]
		public void Seed()
		{
			var type = (TimeAsTimeSpanType) NHibernateUtil.TimeAsTimeSpan;
			Assert.IsTrue(type.Seed(null) is TimeSpan, "seed should be TimeSpan");
		}
	}

	[TestFixture]
	public class TimeSpanFixture2 : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "TimeAsTimeSpan"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from TimeAsTimeSpanClass").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void PropertiesHasExpectedType()
		{
			var classMetaData = Sfi.GetClassMetadata(typeof(TimeAsTimeSpanClass));
			Assert.That(
				classMetaData.GetPropertyType(nameof(TimeAsTimeSpanClass.TimeSpanValue)),
				Is.EqualTo(NHibernateUtil.TimeAsTimeSpan));
			Assert.That(
				classMetaData.GetPropertyType(nameof(TimeAsTimeSpanClass.TimeSpanWithScale)),
				Is.EqualTo(TypeFactory.GetTimeAsTimeSpanType(3)));
		}

		[Test]
		public void SavingAndRetrieving()
		{
			var ticks = DateTime.Parse("23:59:59").TimeOfDay;

			var entity = new TimeAsTimeSpanClass
			{
				TimeSpanValue = ticks
			};

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(entity);
				tx.Commit();
			}

			TimeAsTimeSpanClass entityReturned;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				entityReturned = s.CreateQuery("from TimeAsTimeSpanClass").UniqueResult<TimeAsTimeSpanClass>();

				Assert.AreEqual(ticks, entityReturned.TimeSpanValue);
				Assert.AreEqual(entityReturned.TimeSpanValue.Hours, ticks.Hours);
				Assert.AreEqual(entityReturned.TimeSpanValue.Minutes, ticks.Minutes);
				Assert.AreEqual(entityReturned.TimeSpanValue.Seconds, ticks.Seconds);
			}
		}

		[Test]
		public void LowerDigitsAreIgnored()
		{
			if (!Dialect.SupportsDateTimeScale)
				Assert.Ignore("Lower digits cannot be ignored when dialect does not support scale");

			var baseTime = new TimeSpan(0, 17, 55, 24, 548);
			var entity = new TimeAsTimeSpanClass
			{
				TimeSpanWithScale = baseTime.Add(TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond / 3))
			};
			Assert.That(entity.TimeSpanWithScale, Is.Not.EqualTo(baseTime));

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
				var retrieved = s.Load<TimeAsTimeSpanClass>(id);
				Assert.That(retrieved.TimeSpanWithScale, Is.EqualTo(baseTime));
				t.Commit();
			}
		}
	}
}
