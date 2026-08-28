using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Firebird4DialectFixture
	{
		private readonly Firebird4Dialect _dialect = new Firebird4Dialect();

		[Test]
		public void MaxAliasLengthIsRaisedToTheFirebird4Limit()
		{
			Assert.That(_dialect.MaxAliasLength, Is.EqualTo(63));
		}
	}
}
