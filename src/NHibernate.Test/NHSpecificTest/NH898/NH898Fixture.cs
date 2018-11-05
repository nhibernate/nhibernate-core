using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH898
{
	[TestFixture]
	public class NH898Fixture : BugTestCase
	{
		protected override string[] Mappings => new[]
		{
			"ClassA.hbm.xml",
			"ClassBParent.hbm.xml",
			"ClassB.hbm.xml",
			"ClassC.hbm.xml",
		};

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
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
