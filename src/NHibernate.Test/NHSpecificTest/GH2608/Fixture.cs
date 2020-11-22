using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2608
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from PersonalDetails").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void MergeBidiPrimaryKeyOneToOne()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = new Person { Name = "steve" };
				p.Details = new PersonalDetails { SomePersonalDetail = "I have big feet", Person = p };
				s.Merge(p);
				tx.Commit();
			}
		}
	}
}
