using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1978
{
	[TestFixture]
	public class AliasTest : BugTestCase
	{
		[Test]
		public void ShouldReturnPlanFromEmployee()
		{
			using(var s = OpenSession())
			using (var trans = s.BeginTransaction())
			{
				var plan = new _401k {PlanName = "test"};
				s.Save(plan);
				s.Flush();
				s.Refresh(plan);
				var emp = new Employee {EmpName = "name", PlanParent = plan};
				s.Save(emp);

				trans.Rollback();
			}
		}
	}
}