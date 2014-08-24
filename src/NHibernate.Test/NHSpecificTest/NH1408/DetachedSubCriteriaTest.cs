using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1408
{
	[TestFixture]
	public class DetachedSubCriteriaTest : BugTestCase
	{
		[Test]
		public void Test()
		{
			DetachedCriteria criteria = DetachedCriteria.For(typeof (DbResource));
			DetachedCriteria keyCriteria = criteria.CreateCriteria("keys");
			keyCriteria.Add(Restrictions.Eq("Key0", "2"));
			keyCriteria.Add(Restrictions.Eq("Key1", "en"));
			using (ISession session = OpenSession())
			{
				ICriteria icriteria = CriteriaTransformer.Clone(criteria).GetExecutableCriteria(session);
				icriteria.SetFirstResult(0);
				icriteria.SetMaxResults(1);
				icriteria.List<DbResource>();
				// should not throw when parse the criteria
			}
		}
	}
}