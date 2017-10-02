using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for the <see cref="DateTimeType"/>.
	/// </summary>
	[TestFixture]
	[Obsolete]
	public class DateTime2TypeFixture
	{
		[Test]
		public void Next()
		{
			var type = NHibernateUtil.DateTime2;
			object current = DateTime.Now.AddMilliseconds(-1);
			object next = type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTime>().And.GreaterThan(current));
		}

		[Test]
		public void Seed()
		{
			var type = NHibernateUtil.DateTime2;
			Assert.IsTrue(type.Seed(null) is DateTime, "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			var type = NHibernateUtil.DateTime2;

			object value1 = DateTime.Now;
			object value2 = type.DeepCopy(value1, null);

			Assert.AreEqual(value1, value2, "Copies should be the same.");


			value2 = ((DateTime)value2).AddHours(2);
			Assert.IsFalse(value1 == value2, "value2 was changed, value1 should not have changed also.");
		}

		[Test]
		public void EqualityShouldIgnoreKindAndNotIgnoreMillisecond()
		{
			var type = NHibernateUtil.DateTime;
			var localTime = DateTime.Now;
			var unspecifiedKid = new DateTime(localTime.Ticks, DateTimeKind.Unspecified);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
		}

		[Test]
		public void ObsoleteMessage()
		{
			using (var spy = new LogSpy(typeof(TypeFactory)))
			{
				TypeFactory.Basic(NHibernateUtil.DateTime2.Name);
					Assert.That(
						spy.GetWholeLog(),
						Does.Contain($"{NHibernateUtil.DateTime2.Name} is obsolete. Use DateTimeType instead").IgnoreCase);
			}
		}
	}
}
