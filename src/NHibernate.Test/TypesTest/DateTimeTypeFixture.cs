using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for the <see cref="DateTimeType"/>.
	/// </summary>
	[TestFixture]
	public class DateTimeTypeFixture
	{
		[Test]
		public void Next()
		{
			DateTimeType type = (DateTimeType) NHibernateUtil.DateTime;
			object current = DateTime.Parse("2004-01-01");
			object next = type.Next(current, null);

			Assert.IsTrue(next is DateTime, "Next should be DateTime");
			Assert.IsTrue((DateTime) next > (DateTime) current,
						  "next should be greater than current (could be equal depending on how quickly this occurs)");
		}

		[Test]
		public void Seed()
		{
			DateTimeType type = (DateTimeType) NHibernateUtil.DateTime;
			Assert.IsTrue(type.Seed(null) is DateTime, "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			NullableType type = NHibernateUtil.DateTime;

			object value1 = DateTime.Now;
			object value2 = type.DeepCopy(value1, EntityMode.Poco, null);

			Assert.AreEqual(value1, value2, "Copies should be the same.");


			value2 = ((DateTime)value2).AddHours(2);
			Assert.IsFalse(value1 == value2, "value2 was changed, value1 should not have changed also.");
		}

		[Test]
		public void EqualityShouldIgnoreKindAndMillisecond()
		{
			var type = (DateTimeType)NHibernateUtil.DateTime;
			var localTime = DateTime.Now;
			var unspecifiedKid = new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour, localTime.Minute, localTime.Second, 0, DateTimeKind.Unspecified);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
			Assert.That(type.IsEqual(localTime, unspecifiedKid, EntityMode.Poco), Is.True);
		}
	}
}