using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH826
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			ISession session = OpenSession();
			ITransaction transaction = session.BeginTransaction();

			Activity activity = new Activity();
			session.Save(activity);

			ActivitySet activitySet = new ActivitySet();
			activitySet.Activities.Add(activity);
			session.Save(activitySet);

			transaction.Commit();
			session.Close();

			session = OpenSession();
			transaction = session.BeginTransaction();

			// This works:
			//IList<ActivitySet> list = session.CreateQuery("from ActivitySet a where a.Id = 1").List<ActivitySet>();
			//Console.WriteLine("Got it? {0}", list.Count == 1);
			//session.Flush();

			// This does not
			ActivitySet loadedActivitySet = (ActivitySet) session
			                                              	.CreateCriteria(typeof(ActivitySet))
			                                              	.Add(Expression.Eq("Id", activitySet.Id))
			                                              	.UniqueResult();

			session.Flush();

			foreach (object o in loadedActivitySet.Activities)
			{
				session.Delete(o);				
			}
			session.Delete(loadedActivitySet);

			transaction.Commit();
			session.Close();
		}
	}
}