using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH980
{
	[TestFixture]
	public class NH980Fixture : BugTestCase
	{
		[Test]
		public void IdGeneratorShouldUseQuotedTableName()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IdOnly obj = new IdOnly();
				s.Save(obj);
				s.Flush();
				s.Delete(obj);
				t.Commit();
			}
		}
	}
}
