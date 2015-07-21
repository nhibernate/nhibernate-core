using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH898
{
	[TestFixture]
	public class NH898Fixture : BugTestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"NHSpecificTest.NH898.ClassA.hbm.xml",
						"NHSpecificTest.NH898.ClassBParent.hbm.xml",
						"NHSpecificTest.NH898.ClassB.hbm.xml",
						"NHSpecificTest.NH898.ClassC.hbm.xml",
					};
			}
		}

		[Test]
		public void Bug()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				ClassA a = new ClassA();
				s.Save(a);
				t.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				IList l = session.CreateQuery("from ClassA a left join fetch a.B b").List();
				Console.Write(l.ToString());
				t.Commit();
			}

			using(ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from ClassA");
				s.Flush();
				t.Commit();
			}
		}
	}
}
