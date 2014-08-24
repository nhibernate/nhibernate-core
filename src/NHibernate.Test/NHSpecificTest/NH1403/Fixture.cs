using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1403
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			Hobby h = new Hobby("Develop software");
			Person p = new Male("Diego");
			h.Person = p;
			Hobby h1 = new Hobby("Drive Car");
			Person p1 = new Female("Luciana");
			h1.Person = p1;
			object savedIdMale;
			object saveIdFemale;
			using (ISession s = OpenSession())
			using(ITransaction t = s.BeginTransaction())
			{
				savedIdMale = s.Save(h);
				saveIdFemale = s.Save(h1);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				h = s.Get<Hobby>(savedIdMale);
				h1 = s.Get<Hobby>(saveIdFemale);
				Assert.IsTrue(h.Person is Male);
				Assert.IsTrue(h1.Person is Female);
				s.Delete(h);
				s.Delete(h1);
				t.Commit();
			}
		}
	}
}