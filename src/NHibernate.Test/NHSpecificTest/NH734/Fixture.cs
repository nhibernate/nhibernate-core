using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH734
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH734"; }
		}

		[TestAttribute]
		public void LimitProblem()
		{
			using (ISession session = Sfi.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(MyClass));
				criteria.SetMaxResults(100);
				criteria.SetFirstResult(0);
				try
				{
					session.BeginTransaction();
					IList result = criteria.List();
					session.Transaction.Commit();
				}
				catch
				{
					if (session.Transaction != null)
					{
						session.Transaction.Rollback();
					}
					throw;
				}
			}
		}
	}
}