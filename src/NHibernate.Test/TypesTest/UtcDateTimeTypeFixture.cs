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
	public class UtcDateTimeNoMsTypeFixture : DateTimeNoMsTypeFixture
	{
		protected override string TypeName => "UtcDateTimeNoMs";
		protected override AbstractDateTimeType Type => NHibernateUtil.UtcDateTimeNoMs;
	}
}
