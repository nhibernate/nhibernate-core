using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the DecimalType
	/// </summary>
	[TestFixture]
	public class DecimalTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "Decimal";

		private readonly DecimalType _type = NHibernateUtil.Decimal;
		private const int _highScaleId = 2;
		private const decimal _highScaleTestedValue = 123456789.123456789m;

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return base.AppliesTo(dialect) && !TestDialect.HasBrokenDecimalType;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			if (Dialect is FirebirdDialect)
			{
				configuration.SetProperty(Environment.QueryDefaultCastPrecision, "18");
				configuration.SetProperty(Environment.QueryDefaultCastScale, "9");
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new DecimalClass { Id = _highScaleId, HighScaleDecimalValue = _highScaleTestedValue });
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from DecimalClass").ExecuteUpdate();
				t.Commit();
			}
		}

		/// <summary>
		/// Test that two decimal fields that are exactly equal are returned
		/// as Equal by the DecimalType.
		/// </summary>
		[Test]
		public void Equals()
		{
			const decimal lhs = 5.64351M;
			var rhs = 5.64351M;

			Assert.That(_type.IsEqual(lhs, rhs), Is.True);

			// Test that two decimal fields that are equal except one has a higher precision than
			// the other one are returned as Equal by the DecimalType.
			rhs = 5.643510M;
			Assert.That(_type.IsEqual(lhs, rhs), Is.True);
		}

		[Test]
		public void ReadWrite()
		{
			const decimal expected = 5.64351M;

			var basic = new DecimalClass
			{
				Id = 1,
				DecimalValue = expected
			};

			using (var s = OpenSession())
			{
				s.Save(basic);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				basic = s.Load<DecimalClass>(1);

				Assert.That(basic.DecimalValue, Is.EqualTo(expected));
				Assert.That(basic.DecimalValue, Is.EqualTo(5.643510M));
			}
		}

		[Test]
		public void UnsavedValue()
		{
			var mappedValue = _type.StringToObject("0");
			Assert.That(mappedValue, Is.EqualTo(0m));
			Assert.IsTrue(_type.IsEqual(mappedValue, 0m), "'0' in the mapping file should have been converted to a 0m");
		}

		[Test]
		public void HighScaleParameterSelect()
		{
			if (TestDialect.HasBrokenTypeInferenceOnSelectedParameters)
				Assert.Ignore("Current dialect does not support this test");

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var selectedValue = s
					.CreateQuery("select dc.HighScaleDecimalValue + :d1 from DecimalClass dc")
					.SetMaxResults(1)
					.SetDecimal("d1", _highScaleTestedValue)
					.UniqueResult<object>();
				Assert.That(selectedValue, Is.EqualTo(_highScaleTestedValue * 2));
				t.Commit();
			}
		}

		[Test]
		public void HighScaleParameterFilter()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.CreateQuery("select count(*) from DecimalClass dc where dc.HighScaleDecimalValue = :d1")
					.SetDecimal("d1", _highScaleTestedValue)
					.UniqueResult<long>();
				Assert.That(count, Is.GreaterThanOrEqualTo(1));
				t.Commit();
			}
		}

		[Test]
		public void HighScaleParameterInequality()
		{
			if (!TestDialect.SupportsNonDataBoundCondition)
				Assert.Ignore("Dialect does not support parameters comparison.");

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s
					.CreateQuery("select count(*) from DecimalClass dc where :d1 != :d2")
					.SetDecimal("d1", 123456789.123456789m)
					// If truncation occurs before the last digit, the test will fail.
					.SetDecimal("d2", 123456789.123456780m)
					.UniqueResult<long>();
				Assert.That(count, Is.GreaterThanOrEqualTo(1));
				t.Commit();
			}
		}
	}
}
