using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class UtcTicksTypeFixture : TicksTypeFixture
	{
		protected override string TypeName => "UtcTicks";
		protected override AbstractDateTimeType Type => NHibernateUtil.UtcTicks;
	}
}
