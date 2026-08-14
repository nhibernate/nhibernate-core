using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Firebird4DialectFixture
	{
		private readonly Firebird4Dialect _dialect = new Firebird4Dialect();

		[Test]
		public void GetCreateSequenceStringWithInitialValueAndIncrement()
		{
			// Not lowered by the increment, unlike Firebird 3.
			Assert.That(
				_dialect.GetCreateSequenceStrings("fish_seq", 10, 5),
				Is.EqualTo(new[] { "create sequence fish_seq start with 10 increment by 5" }));
		}
	}
}
