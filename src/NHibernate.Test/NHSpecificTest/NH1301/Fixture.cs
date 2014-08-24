using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1301
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1301"; }
		}

		[Test]
		public void Test()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				ClassA a = new ClassA();
				a.BCollection.Add(new ClassB());
				s.Save(a);
				s.Flush();
				s.Clear();

				//dont know if proxy should be able to refresh
				//so I eager/join load here just to show it doesn't work anyhow...
				ClassA loaded = s.CreateCriteria(typeof(ClassA))
												.SetFetchMode("BCollection", FetchMode.Join)
												.List<ClassA>()[0];
				Assert.AreEqual(1, a.BCollection.Count);
				loaded.BCollection.RemoveAt(0);
				Assert.AreEqual(0, loaded.BCollection.Count);
				s.Refresh(loaded);
				Assert.AreEqual(1, loaded.BCollection.Count);
				s.Delete(loaded);
				tx.Commit();
			}
		}
	}
}
