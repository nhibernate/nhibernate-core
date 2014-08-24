using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1250
{
	/// <summary>
	/// http://jira.nhibernate.org/browse/NH-1250
	/// http://jira.nhibernate.org/browse/NH-1340
	/// </summary>
	/// <remarks>Failure occurs in MsSql2005Dialect only</remarks>
	[TestFixture]
	public class PolymorphicJoinFetchFixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1250"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void FetchUsingICriteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateCriteria(typeof(Party))
					.SetMaxResults(10)
					.List();
				tx.Commit();
			}
		}

		[Test]
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
