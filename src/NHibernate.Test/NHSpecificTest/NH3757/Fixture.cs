using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3757
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.CreateSQLQuery("delete from EntityName").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		[Test]
		public void ShouldBePossibleToHaveComponentInEntityNameMappedEntity()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Dictionary<string, object>();
				e1["Money"] = new Money { Amount = 100m, Currency = "USD" };
				session.Save("EntityName", e1);

				session.Flush();
				transaction.Commit();
			}
		}
	}
}
