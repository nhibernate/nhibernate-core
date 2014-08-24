using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1928
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
        [Test]
        public void SqlCommentAtBeginningOfLine()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                var query = session.CreateSQLQuery(
                    @"
select 1
from 
    Customer 
where
-- this is a comment
    Name = 'Joe'
    and Age > 50
");
   
                Assert.DoesNotThrow(() => query.List());
                tx.Commit();
            }
        }

		[Test]
		public void SqlCommentAtBeginningOfLastLine()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var query = session.CreateSQLQuery(
					@"
select 1
from 
    Customer 
where
    Name = 'Joe'
    and Age > 50
-- this is a comment");

				Assert.DoesNotThrow(() => query.List());
				tx.Commit();
			}
		}

        [Test]
        public void SqlCommentAfterBeginningOfLine()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                var query = session.CreateSQLQuery(
                    @"
select 1
from 
    Customer 
where
 -- this is a comment
    Name = 'Joe'
    and Age > 50
");

                Assert.DoesNotThrow(() => query.List());
                tx.Commit();
            }
        }
    }
}
