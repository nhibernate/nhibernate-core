using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1250
{
	/// <summary>
	/// http://jira.nhibernate.org/browse/NH-1250
	/// </summary>
	/// <remarks>Failure occurs in MsSql2005Dialect only</remarks>
	[TestFixture]
	public class PolymorphicJoinFetchFixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1250"; }
		}

		[Test]
		[Ignore("Not yet fixed")]
		public void FetchUsingICriteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateCriteria(typeof (Party))
					.SetMaxResults(10)
					.List();
				tx.Commit();
			}
		}

		[Test]
		[Ignore("Not yet fixed")]
		public void FetchUsingIQuery()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("from Party")
					.SetMaxResults(10)
					.List();
				tx.Commit();
			}
		}
	}
}
