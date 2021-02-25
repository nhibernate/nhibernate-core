using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH734
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[TestAttribute]
		public void LimitProblem()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				ICriteria criteria = session.CreateCriteria(typeof(MyClass));
				criteria.SetMaxResults(100);
				criteria.SetFirstResult(0);
				IList result = criteria.List();
				tran.Commit();
			}
		}
	}
}
