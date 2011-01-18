using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2341
{
	public class Fixture: BugTestCase
	{
		[Test]
		public void WhenSaveInstanceOfConcreteInheritedThenNotThrows()
		{
			using(var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new ConcreteB();
				session.Executing(s=> s.Save(entity)).NotThrows();
				tx.Commit();
			}
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from AbstractBA").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}