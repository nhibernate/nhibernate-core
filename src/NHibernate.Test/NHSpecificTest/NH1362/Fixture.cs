using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1362
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				ClassA a = new ClassA();
				ClassB b = new ClassB();
				ClassC c = new ClassC();
				a.B = b;
				a.B.CCollection.Add(c);
				s.Save(a);
				s.Flush();
				s.Clear();

				ClassA loaded = s.Load<ClassA>(a.Id);
                
				//work with first child object
				loaded.B = null;
				s.Refresh(loaded);
				Assert.IsNotNull(loaded);
				Assert.AreEqual(1, loaded.B.CCollection.Count);

				//doesn't work with nested object
				loaded.B.CCollection.Clear();
				s.Refresh(loaded);
				Assert.AreEqual(1, loaded.B.CCollection.Count);

				s.Delete(loaded);
				tx.Commit();
			}
		}
	}
}