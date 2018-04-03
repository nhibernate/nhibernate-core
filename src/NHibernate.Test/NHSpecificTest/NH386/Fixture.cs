using System;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH386
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH386"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is AbstractHanaDialect); // HANA does not support inserting a row without specifying any column values
		}

		[Test]
		public void Query()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("from _Parent _p left join _p._Children _c where _c._Id > 0")
					.List();
			}
		}
	}
}
