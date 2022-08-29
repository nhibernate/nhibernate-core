using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for changing a default .Net type.
	/// </summary>
	[TestFixture]
	public class ChangeDefaultTypeWithPrecisionFixture : TypeFixtureBase
	{
		public class CustomCurrencyType : DecimalType
		{
			public CustomCurrencyType() : base(SqlTypeFactory.Currency)
			{
			}

			public CustomCurrencyType(byte precision, byte scale) : base(new SqlType(DbType.Currency, precision, scale))
			{
			}

			public override string Name => "CustomCurrencyType";
		}

		protected override string TypeName => "ChangeDefaultType";

		private IType _originalDefaultType;
		private IType _testDefaultType;
		private static System.Type _replacedType = typeof(decimal);

		protected override void Configure(Configuration configuration)
		{
			_originalDefaultType = TypeFactory.GetDefaultTypeFor(_replacedType);
			_testDefaultType = new CustomCurrencyType();
			Assert.That(_originalDefaultType, Is.Not.Null);
			Assert.That(_originalDefaultType, Is.Not.EqualTo(_testDefaultType));

			TypeFactory.RegisterType(
				_replacedType,
				_testDefaultType,
				new[] { "currency" },
				(precision, scale) => new CustomCurrencyType(precision, scale));
			base.Configure(configuration);
		}

		protected override void DropSchema()
		{
			base.DropSchema();
			TypeFactory.ClearCustomRegistrations();
			Assert.That(TypeFactory.GetDefaultTypeFor(_replacedType), Is.Not.EqualTo(_testDefaultType));
		}

		[Test]
		public void DefaultType()
		{
			Assert.That(TypeFactory.GetDefaultTypeFor(_replacedType), Is.EqualTo(_testDefaultType));
		}

		[Test]
		public void PropertyType()
		{
			var propertyType1 = Sfi.GetClassMetadata(typeof(ChangeDefaultTypeClass))
									.GetPropertyType(nameof(ChangeDefaultTypeClass.CurrencyTypeExplicitPrecision6And3));
			Assert.That(
				propertyType1.GetType(),
				Is.EqualTo(_testDefaultType.GetType()));
			Assert.That(propertyType1.SqlTypes(Sfi)[0].Precision, Is.EqualTo(6));
			Assert.That(propertyType1.SqlTypes(Sfi)[0].Scale, Is.EqualTo(3));

			var propertyType2 = Sfi.GetClassMetadata(typeof(ChangeDefaultTypeClass))
									.GetPropertyType(nameof(ChangeDefaultTypeClass.CurrencyTypePrecisionInType5And2));

			Assert.That(
				propertyType2.GetType(),
				Is.EqualTo(_testDefaultType.GetType()));
			Assert.That(propertyType2.SqlTypes(Sfi)[0].Precision, Is.EqualTo(5));
			Assert.That(propertyType2.SqlTypes(Sfi)[0].Scale, Is.EqualTo(2));
		}

		[Test]
		public void GuessType()
		{
			Assert.That(NHibernateUtil.GuessType(_replacedType), Is.EqualTo(_testDefaultType));
		}

		[Test]
		public void ParameterType()
		{
			var namedParametersField = typeof(AbstractQueryImpl)
				.GetField("namedParameters", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(namedParametersField, Is.Not.Null, "Missing internal field");

			using (var s = OpenSession())
			{
				// Query where the parameter type cannot be deducted from compared entity property
				var q = s.CreateQuery($"from {nameof(ChangeDefaultTypeClass)} where :str1 = :str2")
						.SetParameter("str1", 1.22m)
						.SetDecimal("str2", 1m);

				var namedParameters = namedParametersField.GetValue(q) as Dictionary<string, TypedValue>;
				Assert.That(namedParameters, Is.Not.Null, "Unable to retrieve parameters internal field");
				Assert.That(namedParameters["str1"].Type, Is.EqualTo(_testDefaultType));
				Assert.That(namedParameters["str2"].Type, Is.EqualTo(NHibernateUtil.Decimal));
			}
		}
	}
}
