using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2366
{
	[Ignore("Not fixed yet.")]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				// Note: non-unique values for Value property
				session.Save(new Two() { Id = 1, Value = "a" });
				session.Save(new Two() { Id = 2, Value = "b" });
				session.Save(new Two() { Id = 3, Value = "a" });
				transaction.Commit();
			}
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(new One() { Id = 1, Value = "a" });
				session.Save(new One() { Id = 2, Value = "a" });
				transaction.Commit();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from One");
				session.Delete("from Two");
				
				transaction.Commit();
			}
			
			base.OnTearDown();
		}

		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			{
				session.Executing(s=> s.CreateQuery("from One").List()).NotThrows();
			}
		}
	}
}
