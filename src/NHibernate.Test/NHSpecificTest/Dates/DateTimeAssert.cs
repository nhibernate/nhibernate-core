using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	/// <summary>
	/// Useful assert to work with datetimes types.
	/// </summary>
	public class DateTimeAssert
	{
		public static void AreEqual(DateTime dt1, DateTime dt2)
		{
			bool areEqual = new DateTimeType().IsEqual(dt1, dt2);

			if (!areEqual)
				Assert.Fail(string.Format("Expected {0} but was {1}", dt1, dt2));
		}

		public static void AreEqual(DateTime dt1, DateTime dt2, bool OnlyDatePart)
		{
			if (!OnlyDatePart)
				throw new NotSupportedException();

			bool areEqual = DatePartAreEqual(dt1, dt2);

			if (!areEqual)
				Assert.Fail(string.Format("Expected {0} but was {1}", dt1, dt2));
		}


		public static void AreEqual(DateTimeOffset dt1, DateTimeOffset dt2)
		{
			bool areEqual = new DateTimeOffsetType().IsEqual(dt1, dt2);

			if (!areEqual)
				Assert.Fail(string.Format("Expected {0} but was {1}", dt1, dt2));
		}

		/// <summary>
		/// Compare only the date part.
		/// </summary>
		/// <returns>true if are equal, false otherwise</returns>
		private static bool DatePartAreEqual(DateTime x, DateTime y)
		{
			if (x == y)
			{
				return true;
			}

			DateTime date1 = x;
			DateTime date2 = y;

			if (date1.Equals(date2))
				return true;

			return (date1.Year == date2.Year &&
			        date1.Month == date2.Month &&
			        date1.Day == date2.Day);
		}
	}
}