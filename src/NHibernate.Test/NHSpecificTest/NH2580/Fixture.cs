using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2580
{
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
				var exeption = s.Executing(x=> x.Get<MyClass>(1)).Throws<HibernateException>().Exception;
				exeption.Message.ToLowerInvariant().Should().Contain("possible cause");
			}
		}
	}
}