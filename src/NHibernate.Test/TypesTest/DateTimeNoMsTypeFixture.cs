using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for the <see cref="DateTimeNoMsType"/>.
	/// </summary>
	[TestFixture]
	public class DateTimeNoMsTypeFixture
	{
		[Test]
		public void Next()
		{
			var type = NHibernateUtil.DateTimeNoMs;
			object current = DateTime.Parse("2004-01-01");
			object next = type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTime>(), "next should be DateTime");
			Assert.That(next, Is.GreaterThan(current), "next should be greater than current");
		}

		[Test]
		public void Seed()
		{
			var type = NHibernateUtil.DateTimeNoMs;
			Assert.That(type.Seed(null), Is.TypeOf<DateTime>(), "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			NullableType type = NHibernateUtil.DateTimeNoMs;

			object value1 = DateTime.Now;
			object value2 = type.DeepCopy(value1, null);

			Assert.That(value2, Is.EqualTo(value1), "Copies should be the same.");

			value2 = ((DateTime) value2).AddHours(2);
			Assert.That(value2, Is.Not.EqualTo(value1), "value2 was changed, value1 should not have changed also.");
		}

		[Test]
		public void EqualityShouldIgnoreKindAndMillisecond()
		{
			var type = NHibernateUtil.DateTimeNoMs;
			var localTime = DateTime.Now;
			var unspecifiedKid = new DateTime(
				localTime.Year,
				localTime.Month,
				localTime.Day,
				localTime.Hour,
				localTime.Minute,
				localTime.Second,
				0,
				DateTimeKind.Unspecified);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
		}
	}
}
