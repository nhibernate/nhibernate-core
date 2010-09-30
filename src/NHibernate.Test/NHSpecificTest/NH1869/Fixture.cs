using System.Collections;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1869
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Keyword _keyword;

		protected override void OnTearDown()
		{
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete("from NodeKeyword");
					session.Delete("from Keyword");
					transaction.Commit();
				}
			}
		}

		[Test]
		public void Test()
		{
			IDriver driver = sessions.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
			{
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
			}
			
			using (ISession session = sessions.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				_keyword = new Keyword();
				session.Save(_keyword);

				NodeKeyword nodeKeyword = new NodeKeyword();
				nodeKeyword.NodeId = 1;
				nodeKeyword.Keyword = _keyword;
				session.Save(nodeKeyword);

				transaction.Commit();
			}

			using (ISession session = sessions.OpenSession())
			{
				//If uncomment the line below the test will pass
				//GetResult(session);
				IList result = GetResult(session);
				Assert.That(result, Has.Count.EqualTo(2));
				Assert.That(result[0], Has.Count.EqualTo(1));
				Assert.That(result[1], Has.Count.EqualTo(1));
			}
		}

		private IList GetResult(ISession session)
		{
			IQuery query1 = session.CreateQuery("from NodeKeyword nk");
			IQuery query2 = session.CreateQuery("from NodeKeyword nk");

			var multi = session.CreateMultiQuery();
			multi.Add(query1).Add(query2);
			return multi.List();
		}
	}
}