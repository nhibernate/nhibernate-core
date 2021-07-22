using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2621Enum
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestOk()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2621Enum.ClassWithString ROOT WHERE ROOT.Kind = :kind");
				query.SetParameter("kind", Kind.SomeKind);
				Assert.DoesNotThrow(() => query.List());
			}
		}
	}
}
