using System;

namespace NHibernate.Test
{
	using NUnit.Framework;

	[TestFixture]
	public class NHibernateUtilTest
	{
		[Test]
		public void CanGuessTypeOfInt32ByValue()
		{
			Assert.That(NHibernateUtil.GuessType(15), Is.EqualTo(NHibernateUtil.Int32));
		}

		[Test]
		public void CanGuessTypeOfInt32ByType()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(int)), Is.EqualTo(NHibernateUtil.Int32));
		}

		[Test]
		public void CanGuessTypeOfNullableInt32ByType()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(int?)), Is.EqualTo(NHibernateUtil.Int32));
		}

		[Test]
		public void CanGuessTypeOfNullableInt32ByValue()
		{
			int? val = 15;
			Assert.That(NHibernateUtil.GuessType(val), Is.EqualTo(NHibernateUtil.Int32));
		}

		[Test]
		public void CanGuessTypeOfDateTime()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(DateTime)), Is.EqualTo(NHibernateUtil.DateTime));
		}

		[Test]
		public void CanGuessTypeOfString()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(string)), Is.EqualTo(NHibernateUtil.String));
		}

		[Test]
		public void CanGuessTypeOfBoolean()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(bool)), Is.EqualTo(NHibernateUtil.Boolean));
		}

		[Test]
		public void CanGuessTypeOfDecimal()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(decimal)), Is.EqualTo(NHibernateUtil.Decimal));
		}

		[Test]
		public void CanGuessTypeOfTimeSpan()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(TimeSpan)), Is.EqualTo(NHibernateUtil.TimeSpan));
		}
	}
}
