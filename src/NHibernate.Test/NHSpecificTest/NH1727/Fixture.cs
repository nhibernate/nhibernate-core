using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1727
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		/* To the commiter
         * I'm using sql2005dialect
         * From what I've read there's been some diffucalties with this
         * dialect before when used parameter queries.
         * The first test (xxx_DoesNotWorkToday) passed in NH 2.0
         * The second test passes where I've just switched the order in the where clause
          */


		[Test]
		public void VerifyFilterAndInAndProperty_DoesNotWorkToday()
		{
			const string hql = @"select a from ClassA a 
                                    where a.Value in (:aValues)
                                        and a.Name=:name";
			ClassB b = new ClassB();
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(b);
				t.Commit();
			}
			using (ISession s = OpenSession())
			{
				s.EnableFilter("bEquals").SetParameter("b", b.Id);
				s.CreateQuery(hql)
					.SetString("name", "Sweden")
					.SetParameterList("aValues", new[] { 1, 3, 4 })
					.List<ClassA>();
			}
		}


		[Test]
		public void VerifyFilterAndInAndProperty_WorksToday()
		{
			const string hql = @"select a from ClassA a 
                                    where a.Name=:name
                                        and a.Value in (:aValues)";
			ClassB b = new ClassB();
			using(ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(b);
				t.Commit();
			}
			using(ISession s = OpenSession())
			{
				s.EnableFilter("bEquals").SetParameter("b", b.Id);
				s.CreateQuery(hql)
					.SetString("name", "Sweden")
					.SetParameterList("aValues", new []{1,3,4})
					.List<ClassA>();
			}
		}

		protected override void OnTearDown()
		{
			using(ISession s = OpenSession())
			using(ITransaction t = s.BeginTransaction())
			{
				s.Delete("from ClassB");
				s.Delete("from ClassA");
				t.Commit();
			}
		}
        
	}
}