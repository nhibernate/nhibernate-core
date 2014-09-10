using System;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.TypesTest
{
    /// <summary>
    /// TestFixtures for the <see cref="DateTimeType"/>.
    /// </summary>
    [TestFixture]
    public class DateTime2TypeFixture
    {
        [Test]
        public void Next()
        {
            DateTimeType type = (DateTimeType)NHibernateUtil.DateTime2;
            object current = DateTime.Now.AddMilliseconds(-1);
            object next = type.Next(current, null);

						next.Should().Be.OfType<DateTime>().And.Value.Should().Be.GreaterThan((DateTime)current);
        }

        [Test]
        public void Seed()
        {
            DateTimeType type = (DateTimeType)NHibernateUtil.DateTime;
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
				public void EqualityShouldIgnoreKindAndNotIgnoreMillisecond()
				{
					var type = (DateTimeType)NHibernateUtil.DateTime;
					var localTime = DateTime.Now;
					var unspecifiedKid = new DateTime(localTime.Ticks, DateTimeKind.Unspecified);
					type.Satisfy(t => t.IsEqual(localTime, unspecifiedKid));
					type.Satisfy(t => t.IsEqual(localTime, unspecifiedKid, EntityMode.Poco));
				}
    }
}