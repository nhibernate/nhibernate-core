using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	public class DateTimeAssert
	{
		public static void AreEqual(DateTime dt1, DateTime dt2)
		{
			bool areEqual = new DateTimeType().IsEqual(dt1, dt2);

			if (!areEqual)
				Assert.Fail(string.Format("Expected {0} but was {1}",dt1,dt2));
		}

		public static void AreEqual(DateTimeOffset dt1, DateTimeOffset dt2)
		{
			bool areEqual = new DateTimeOffsetType().IsEqual(dt1, dt2);

			if (!areEqual)
				Assert.Fail(string.Format("Expected {0} but was {1}", dt1, dt2));
		}
	}
}