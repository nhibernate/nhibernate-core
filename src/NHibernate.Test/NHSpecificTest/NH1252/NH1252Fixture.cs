using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1252
{
	/// <summary>
	/// http://jira.nhibernate.org/browse/NH-1252
	/// </summary>
	[TestFixture]
	public class NH1252Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1252"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from SomeClass");
				tx.Commit();
			}
		}

		[Test]
		[Ignore("Not yet fixed")]
		public void Test()
		{
			SubClass1 sc1 = new SubClass1();
			sc1.Name = "obj1";

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(sc1);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Assert.IsNull(s.Get<SubClass2>(sc1.Id));
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Load<SomeClass>(sc1.Id); // Load a proxy by the parent class

				Assert.IsNull(s.Get<SubClass2>(sc1.Id));
				tx.Commit();
			}

		}
	}
}
