using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for changing a default .Net type.
	/// </summary>
	[TestFixture]
	public class ChangeDefaultTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "ChangeDefaultType";

		private IType _originalDefaultDateTimeType;
		private IType _testDefaultDateTimeType;

		protected override void Configure(Configuration configuration)
		{
			_originalDefaultDateTimeType = TypeFactory.GetDefaultTypeFor(typeof(DateTime));
			Assert.That(_originalDefaultDateTimeType, Is.Not.Null);
			_testDefaultDateTimeType = NHibernateUtil.DateTime.Equals(_originalDefaultDateTimeType)
				? (IType) NHibernateUtil.Timestamp
				: NHibernateUtil.DateTime;
			TypeFactory.SetDefaultType<DateTime>(_testDefaultDateTimeType);
			base.Configure(configuration);
		}

		protected override void DropSchema()
		{
			base.DropSchema();
			if (_originalDefaultDateTimeType != null)
				TypeFactory.SetDefaultType<DateTime>(_originalDefaultDateTimeType);
		}

		[Test]
		public void DefaultType()
		{
			Assert.That(TypeFactory.GetDefaultTypeFor(typeof(DateTime)), Is.EqualTo(_testDefaultDateTimeType));
		}

		[Test]
		public void PropertyType()
		{
			Assert.That(
				Sfi.GetClassMetadata(typeof(ChangeDefaultTypeClass))
				   .GetPropertyType(nameof(ChangeDefaultTypeClass.NormalDateTimeValue)),
				Is.EqualTo(_testDefaultDateTimeType));
		}

		[Test]
		public void GuessType()
		{
			Assert.That(NHibernateUtil.GuessType(typeof(DateTime)), Is.EqualTo(_testDefaultDateTimeType));
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
				var q = s.CreateQuery($"from {nameof(ChangeDefaultTypeClass)} where :date1 = :date2 or :date1 = :date3")
				         .SetParameter("date1", DateTime.Now)
				         .SetDateTime("date2", DateTime.Now)
				         .SetTimestamp("date3", DateTime.Now);

				var namedParameters = namedParametersField.GetValue(q) as Dictionary<string, TypedValue>;
				Assert.That(namedParameters, Is.Not.Null, "Unable to retrieve parameters internal field");
				Assert.That(namedParameters["date1"].Type, Is.EqualTo(_testDefaultDateTimeType));
				Assert.That(namedParameters["date2"].Type, Is.EqualTo(NHibernateUtil.DateTime));
				Assert.That(namedParameters["date3"].Type, Is.EqualTo(NHibernateUtil.Timestamp));
			}
		}
	}
}
