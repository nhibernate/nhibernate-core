using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH593
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (ISession session = OpenSession())
			{
				User user = new User("test");
				user.UserId = 10;
				Assert.Throws<QueryException>(() => session.CreateCriteria(typeof(Blog))
					.Add(Expression.In("Users", new User[] {user}))
					.List());
			}
		}
	}
}