using System.Collections;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1869
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Keyword _keyword;

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
		   return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from NodeKeyword");
				session.Delete("from Keyword");

				transaction.Commit();
			}
		}

		[Test]
		public void Test()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				_keyword = new Keyword();
				session.Save(_keyword);

				var nodeKeyword = new NodeKeyword();
				nodeKeyword.NodeId = 1;
				nodeKeyword.Keyword = _keyword;
				session.Save(nodeKeyword);

				transaction.Commit();
			}

			using (var session = Sfi.OpenSession())
			{
				//If uncomment the line below the test will pass
				//GetResult(session);
				var result = GetResult(session);
				Assert.That(result, Has.Count.EqualTo(2));
				Assert.That(result[0], Has.Count.EqualTo(1));
				Assert.That(result[1], Has.Count.EqualTo(1));
			}
		}

		private IList GetResult(ISession session)
		{
			var query1 = session.CreateQuery("from NodeKeyword nk");
			var query2 = session.CreateQuery("from NodeKeyword nk");

			var multi = session.CreateMultiQuery();
			multi.Add(query1).Add(query2);
			return multi.List();
		}
	}
}