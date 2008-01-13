using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH593
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test, ExpectedException(typeof(QueryException))]
		public void Bug()
		{
			using (ISession session = OpenSession())
			{
				User user = new User("test");
				user.UserId = 10;
				session.CreateCriteria(typeof(Blog))
					.Add(Expressions.Expression.In("Users", new User[] {user}))
					.List();
			}
		}
	}
}