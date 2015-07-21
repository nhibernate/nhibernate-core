using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1574
{
	[TestFixture]
	public class StatelessTest : BugTestCase
	{
		[Test]
		public void StatelessManyToOne()
		{
			using (ISession session = OpenSession())
			{
				var principal = new SpecializedPrincipal();
				var team = new SpecializedTeamStorage();
				principal.Team = team;
				session.SaveOrUpdate(team);
				session.SaveOrUpdate(principal);
				session.Flush();
			}

			using (IStatelessSession session = sessions.OpenStatelessSession())
			{
				IQuery query = session.CreateQuery("from SpecializedPrincipal p");
				IList<Principal> principals = query.List<Principal>();
				Assert.AreEqual(1, principals.Count);

				ITransaction trans = session.BeginTransaction();
				foreach (var principal in principals)
				{
					principal.Name = "Buu";
					session.Update(principal);
				}
				trans.Commit();
			}

			// cleanup
			using (ISession session = OpenSession())
			{
				session.Delete("from SpecializedTeamStorage");
				session.Delete("from SpecializedPrincipal");
				session.Flush();
			}
		}
	}
}