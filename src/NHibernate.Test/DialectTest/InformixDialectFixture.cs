using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class InformixDialectFixture
	{
		private readonly InformixDialect _dialect = new InformixDialect();

		[Test]
		public void ToBooleanValueStringTrue()
		{
			Assert.That(_dialect.ToBooleanValueString(true), Is.EqualTo("'t'"));
		}

		[Test]
		public void ToBooleanValueStringFalse()
		{
			Assert.That(_dialect.ToBooleanValueString(false), Is.EqualTo("'f'"));
		}
	}
}
