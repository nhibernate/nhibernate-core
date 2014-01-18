using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class FirebirdDialectFixture
	{
		#region Tests

		[Test]
		public void GetLimitString()
		{
			FirebirdDialect d = MakeDialect();

			SqlString str = d.GetLimitString(new SqlString("SELECT * FROM fish"), null, new SqlString("10"));
			Assert.AreEqual("SELECT first 10 * FROM fish", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("5"), new SqlString("15"));
			Assert.AreEqual("SELECT first 15 skip 5 * FROM fish ORDER BY name", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name DESC"), new SqlString("7"),
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 * FROM fish ORDER BY name DESC", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), null,
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), new SqlString("7"),
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());
		}

		[Test]
		public void GetTypeName_DecimalWithoutPrecisionAndScale_ReturnsDecimalWithPrecisionOf18AndScaleOf5()
		{
			var fbDialect = MakeDialect();

			var result = fbDialect.GetTypeName(NHibernateUtil.Decimal.SqlType);

			Assert.AreEqual("DECIMAL(18, 5)", result);
		}

		[Test]
		public void GetTypeName_DecimalWithPrecisionAndScale_ReturnsPrecisedAndScaledDecimal()
		{
			var fbDialect = MakeDialect();

			var result = fbDialect.GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 17, 2);

			Assert.AreEqual("DECIMAL(17, 2)", result);
		}

		[Test]
		public void GetTypeName_DecimalWithPrecisionGreaterThanFbMaxPrecision_ReturnsDecimalWithFbMaxPrecision()
		{
			var fbDialect = MakeDialect();

			var result = fbDialect.GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 19, 2);
				//Firebird allows a maximum precision of 18

			Assert.AreEqual("DECIMAL(18, 2)", result);
		}

		#endregion

		#region Private Members

		private static FirebirdDialect MakeDialect()
		{
			return new FirebirdDialect();
		}

		#endregion
	}
}