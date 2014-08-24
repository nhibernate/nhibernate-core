using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1834
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			var a = new A {Id = 1};
			var a2 = new A {Id = 2};
			var b = new B {Id = 1};

			using (ISession session = base.OpenSession())
			{
				session.Save(a);
				session.Save(a2);
				session.Save(b);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = base.OpenSession())
			{
				session.Delete("from B");
				session.Delete("from A");
				session.Flush();
			}
		}

		[Test]
		public void OneToManyPropertyWithFormulaNodeShouldWorkLikeFormulaAttrib()
		{
			using (ISession session = base.OpenSession())
			{
				session.Clear();

				var b = session.Get<B>(1);
				Assert.IsNotNull(b.A2);
				Assert.IsNotNull(b.A);
				Assert.That(b.A.Id == b.A2.Id);
			}
		}
	}
}