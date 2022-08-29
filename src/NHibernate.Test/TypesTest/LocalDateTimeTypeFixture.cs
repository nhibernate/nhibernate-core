using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the LocalDateTimeType.
	/// </summary>
	[TestFixture]
	public class LocalDateTimeTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "LocalDateTime";
		protected override AbstractDateTimeType Type => NHibernateUtil.LocalDateTime;
	}

	[TestFixture]
	public class LocalDateTimeTypeWithScaleFixture : DateTimeTypeWithScaleFixture
	{
		protected override string TypeName => "LocalDateTimeWithScale";
		protected override AbstractDateTimeType Type => (AbstractDateTimeType) TypeFactory.GetLocalDateTimeType(3);
	}

	[TestFixture]
	public class LocalDateTimeNoMsTypeFixture : DateTimeNoMsTypeFixture
	{
		protected override string TypeName => "LocalDateTimeNoMs";
		protected override AbstractDateTimeType Type => NHibernateUtil.LocalDateTimeNoMs;
	}
}
