using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1908
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		[Test]
		public void QueryPropertyInBothFilterAndQuery()
		{
			using (ISession s = OpenSession())
			{
				s.EnableFilter("validity")
					.SetParameter("date", DateTime.Now);

				s.CreateQuery(@"
				select 
					inv.ID
				from 
					Invoice inv
						join inv.Category cat with cat.ValidUntil > :now
						left join cat.ParentCategory parentCat
				where
					inv.ID = :invId
					and inv.Issued < :now
				")
					.SetDateTime("now", DateTime.Now)
					.SetInt32("invId", -999)
					.List();
			}
		}

        [Test]
        public void QueryPropertyInBothFilterAndQueryUsingWith()
        {
            using (ISession s = OpenSession())
            {
                s.EnableFilter("validity")
                    .SetParameter("date", DateTime.Now);

                s.CreateQuery(@"
				select 
					inv.ID
				from 
					Invoice inv
						join inv.Category cat with cat.ValidUntil > :now
						left join cat.ParentCategory parentCat with parentCat.ID != :myInt
				where
					inv.ID = :invId
					and inv.Issued < :now
				")
                    .SetDateTime("now", DateTime.Now)
                    .SetInt32("invId", -999)
                    .SetInt32("myInt", -888)
                    .List();
            }
        }
	}
}