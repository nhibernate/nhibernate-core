using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1763
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		[Test]
		public void CanUseConditionalOnCompositeType()
		{
			using (ISession session = OpenSession())
			{
				session.CreateCriteria<Customer>()
					.SetProjection(Projections.Conditional(Restrictions.IdEq(1),
					                                       Projections.Property("Name"),
					                                       Projections.Property("Name2")))
					.List();
			}
		}
	}
}