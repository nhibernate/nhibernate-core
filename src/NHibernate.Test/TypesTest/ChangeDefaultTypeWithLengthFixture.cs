using System;
using System.Collections.Generic;
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
	public class ChangeDefaultTypeWithLengthFixture : TypeFixtureBase
	{
		public class CustomStringType : AbstractStringType
		{
			public CustomStringType() : base(new StringSqlType())
			{
			}

			public CustomStringType(int length) : base(new StringSqlType(length))
			{
			}

			public override string Name => "CustomStringType";
		}

		protected override string TypeName => "ChangeDefaultType";

		private IType _originalDefaultStringType;
		private IType _testDefaultStringType;
		private static System.Type _replacedType = typeof(string);

		protected override void Configure(Configuration configuration)
		{
			_originalDefaultStringType = TypeFactory.GetDefaultTypeFor(_replacedType);
			Assert.That(_originalDefaultStringType, Is.Not.Null);
			_testDefaultStringType = new CustomStringType();

			TypeFactory.RegisterType(
				_replacedType,
				_testDefaultStringType,
				new[] { "string" },
				length => new CustomStringType(length));
			base.Configure(configuration);
		}

		protected override void DropSchema()
		{
			base.DropSchema();
			TypeFactory.ClearCustomRegistrations();
			Assert.That(TypeFactory.GetDefaultTypeFor(_replacedType), Is.Not.EqualTo(_testDefaultStringType));
		}

		[Test]
		public void DefaultType()
		{
			Assert.That(TypeFactory.GetDefaultTypeFor(_replacedType), Is.EqualTo(_testDefaultStringType));
		}

		[Test]
		public void PropertyType()
		{
			var propertyType25 = Sfi.GetClassMetadata(typeof(ChangeDefaultTypeClass))
								.GetPropertyType(nameof(ChangeDefaultTypeClass.StringTypeLengthInType25));
			Assert.That(
				propertyType25.GetType(),
				Is.EqualTo(_testDefaultStringType.GetType()));
			Assert.That(propertyType25.SqlTypes(Sfi)[0].Length, Is.EqualTo(25));

			var propertyType20 = Sfi.GetClassMetadata(typeof(ChangeDefaultTypeClass))
								.GetPropertyType(nameof(ChangeDefaultTypeClass.StringTypeExplicitLength20));

			Assert.That(
				propertyType20.GetType(),
				Is.EqualTo(_testDefaultStringType.GetType()));
			Assert.That(propertyType20.SqlTypes(Sfi)[0].Length, Is.EqualTo(20));
		}

		[Test]
		public void GuessType()
		{
			Assert.That(NHibernateUtil.GuessType(_replacedType), Is.EqualTo(_testDefaultStringType));
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
				var q = s.CreateQuery($"from {nameof(ChangeDefaultTypeClass)} where :str1 = :str2 or :str1 = :str3")
						.SetParameter("str1", "aaa")
						.SetString("str2", "bbb")
						.SetAnsiString("str3", "bbb");

				var namedParameters = namedParametersField.GetValue(q) as Dictionary<string, TypedValue>;
				Assert.That(namedParameters, Is.Not.Null, "Unable to retrieve parameters internal field");
				Assert.That(namedParameters["str1"].Type, Is.EqualTo(_testDefaultStringType));
				Assert.That(namedParameters["str2"].Type, Is.EqualTo(NHibernateUtil.String));
				Assert.That(namedParameters["str3"].Type, Is.EqualTo(NHibernateUtil.AnsiString));
			}
		}
	}
}
