using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1092
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void CountHasUniqueResult()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Subscriber1 {Username = "u11"});
				s.Save(new Subscriber1 {Username = "u12"});
				s.Save(new Subscriber1 {Username = "u13"});
				s.Save(new Subscriber2 {Username = "u21"});
				s.Save(new Subscriber2 {Username = "u22"});
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var count =
					s.CreateQuery("select count(*) from SubscriberAbstract SA where SA.Username like :username")
					.SetString("username","u%")
					.UniqueResult<long>();
				Assert.That(count, Is.EqualTo(5));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from SubscriberAbstract").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}