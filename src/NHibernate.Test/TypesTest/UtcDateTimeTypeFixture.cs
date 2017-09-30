using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the UtcDateTimeType.
	/// </summary>
	[TestFixture]
	public class UtcDateTimeTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "UtcDateTime";
		protected override AbstractDateTimeType Type => NHibernateUtil.UtcDateTime;
	}

	[TestFixture]
	public class UtcDateTimeTypeWithScaleFixture : DateTimeTypeWithScaleFixture
	{
		protected override string TypeName => "UtcDateTimeWithScale";
		protected override AbstractDateTimeType Type => (AbstractDateTimeType)TypeFactory.GetUtcDateTimeType(3);
	}

	[TestFixture]
	public class UtcDateTimeNoMsTypeFixture : DateTimeNoMsTypeFixture
	{
		protected override string TypeName => "UtcDateTimeNoMs";
		protected override AbstractDateTimeType Type => NHibernateUtil.UtcDateTimeNoMs;
	}
}
