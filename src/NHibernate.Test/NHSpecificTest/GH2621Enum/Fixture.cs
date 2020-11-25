using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2621Enum
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestOk()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var query = s.CreateQuery(@"
							SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2620Enum.ClassWithString ROOT WHERE ROOT.Kind = :kind");
					query.SetParameter("kind", Kind.SomeKind);
					Assert.DoesNotThrow(() => query.List());
				}
			}
		}
	}
}
