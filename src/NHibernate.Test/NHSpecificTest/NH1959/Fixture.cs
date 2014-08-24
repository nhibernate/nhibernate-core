using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1959
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from ClassB");
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public void StartWithEmptyDoAddAndRemove()
		{
			ClassB b = new ClassB();
			ClassA a = new ClassA();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(a);
				s.Save(b);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				ClassA loadedA = s.Get<ClassA>(a.Id);
				ClassB loadedB = s.Get<ClassB>(b.Id);
				loadedA.TheBag.Add(loadedB);
				loadedA.TheBag.Remove(loadedB);
				tx.Commit();
			}

			using (ISession s = OpenSession())
				Assert.AreEqual(0, s.Get<ClassA>(a.Id).TheBag.Count);
		}

		[Test]
		public void StartWithEmptyDoAddAndRemoveAt()
		{
			ClassB b = new ClassB();
			ClassA a = new ClassA();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(a);
				s.Save(b);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				ClassA loadedA = s.Get<ClassA>(a.Id);
				ClassB loadedB = s.Get<ClassB>(b.Id);
				loadedA.TheBag.Add(loadedB);
				loadedA.TheBag.RemoveAt(0);
				tx.Commit();
			}

			using (ISession s = OpenSession())
				Assert.AreEqual(0, s.Get<ClassA>(a.Id).TheBag.Count);
		}
	}
}
