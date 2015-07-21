namespace NHibernate.Test
{
	using NUnit.Framework;

	[TestFixture]
	public class NHibernateUtilTest
	{
		[Test]
		public void CanGuessTypeOfInt32ByValue()
		{
			Assert.AreEqual(NHibernateUtil.Int32, NHibernateUtil.GuessType(15));
		}

		[Test]
		public void CanGuessTypeOfInt32ByType()
		{
			Assert.AreEqual(NHibernateUtil.Int32,
				NHibernateUtil.GuessType(typeof(int)));
		}

		[Test]
		public void CanGuessTypeOfNullableInt32ByType()
		{
			Assert.AreEqual(NHibernateUtil.Int32,
				NHibernateUtil.GuessType(typeof(int?)));
		}


		[Test]
		public void CanGuessTypeOfNullableInt32ByValue()
		{
			int? val = 15;
			Assert.AreEqual(NHibernateUtil.Int32,
				NHibernateUtil.GuessType(val));
		}
	}
}