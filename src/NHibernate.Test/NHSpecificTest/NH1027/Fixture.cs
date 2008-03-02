using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1027
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1027"; }
		}

		private void AssertDialect()
		{
			if (!(Dialect is MsSql2005Dialect))
				Assert.Ignore("This test is specific for MsSql2005Dialect");
		}

		[Test]
		[Ignore("Not completely fixed yet.")]
		public void CanMakeCriteriaQueryAcrossBothAssociations()
		{
			AssertDialect();
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(Item));
				criteria.CreateCriteria("Ships", "s", JoinType.InnerJoin)
							.Add(Expressions.Expression.Eq("s.Id", 15));
				criteria.CreateCriteria("Containers", "c", JoinType.LeftOuterJoin)
				 .Add(Expressions.Expression.Eq("c.Id", 15));
				criteria.SetMaxResults(2);
				criteria.List();
			}
		}
	}
}
