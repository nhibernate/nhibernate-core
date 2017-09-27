using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class TimestampTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "Timestamp";
		protected override AbstractDateTimeType Type => NHibernateUtil.Timestamp;
	}
}
