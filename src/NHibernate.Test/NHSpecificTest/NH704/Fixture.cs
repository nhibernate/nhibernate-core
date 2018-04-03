using System;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH704
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH704"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is AbstractHanaDialect); // HANA does not support inserting a row without specifying any column values
		}

		[Test]
		public void ReAttachCatTest()
		{
			try
			{
				using (ISession sess = OpenSession())
				{
					Cat c = new Cat();
					sess.Save(c);
					sess.Flush();
					sess.Clear();
					sess.Lock(c, LockMode.None); //Exception throw here
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				{
					s.Delete("from Cat");
					s.Flush();
				}
			}
		}
	}
}
