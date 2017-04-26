using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2341
{
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				tx.Commit();
			}
		}

		[Test]
		public void WhenSaveInstanceOfConcreteInheritedThenNotThrows()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new ConcreteB();
				Assert.That(() => session.Save(entity), Throws.Nothing);
				tx.Commit();
			}
		}
	}
}
