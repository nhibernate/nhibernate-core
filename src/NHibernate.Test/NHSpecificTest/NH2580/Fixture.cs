using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2580
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		private class MyClass
		{
		}

		[Test]
		public void WhenPersisterNotFoundShouldThrowAMoreExplicitException()
		{
			using (var s = OpenSession())
			{
				var exeption = Assert.Throws<HibernateException>(() => s.Get<MyClass>(1));
				Assert.That(exeption.Message.ToLowerInvariant(), Does.Contain("possible cause"));
			}
		}
	}
}
